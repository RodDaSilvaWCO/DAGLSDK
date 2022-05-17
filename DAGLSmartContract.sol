// SPDX-License-Identifier: Apache-2.0
pragma solidity >=0.4.9 <0.9.0;

import "./HederaTokenService.sol";
import "./HederaResponseCodes.sol";


contract dAccountingSupport is HederaTokenService {
   function AtomicJournalEntryPost(bytes memory memBytes ) external returns (int response)
   {                
        uint256 accountTransferCount = uint256(uint64(toInt64(memBytes, 56)));       
        uint256 isReversing = uint256(uint64(toInt64(memBytes, 48)));  
        address[] memory transferAddresses = new address[](uint256(uint64(accountTransferCount + 2 )));   // NOTE:  +2 - see comments below
        int64[] memory transferAmounts = new int64[](uint256(uint64(accountTransferCount + 2)));          // NOTE:  +2 - see comments below  
        // First 2 slots in transferAddresses used to store tokenId and treasuryId - hack to reduce number of variables to avoid "Stack too deep" error
        transferAddresses[0] =  toAddress( memBytes, 0);  // Token Id
        transferAddresses[1] = toAddress( memBytes, 20);  // Treasury Id
        // First 2 slots in transferAmounts used to store maxTransactionBatchSize and totalTokensToTransferFromTreasury - hack to reduce number of variables to avoid "Stack too deep" error
        transferAmounts[0] = toInt64(memBytes, 40);       // maxTransactionBatchSize
        transferAmounts[1] = 0;                           // totalTokensToTransferFromTreasury
        uint256 index = 64;                               // We have consumed the first 56 bytes of memBytes, position index read to next byte
        // We need a maxTransactionBatchSize of at least 2 since 1 slot in the transaction is used up by the Treasury leaving a minimum of 1 for the other account
        if( transferAmounts[0] < 2)
        {
            return HederaResponseCodes.INVALID_CHUNK_NUMBER;
        }
        // Loop to parse the GL account address/amount pair from memBytes and store in parallel transferAddresses/transferAmounts arrays
        for( uint256 i = 0; i < accountTransferCount; i++)
        {              
            transferAddresses[i+2]  = toAddress( memBytes, index);      // NOTE: +2 to skip first two reserved slots - see comments above
            index += 20; // size of an EMV address in bytes
            transferAmounts[i+2] = toInt64(memBytes, index);            // NOTE: +2 to skip first two reserved slots - see comments above
            index += 8;  // size of an int64 in bytes
        }
        // The request number of account transfer swaps may exceed the maximum allowable at one time (as specifed by the maxTransactionBatchSize)
        // So we may need to batch the transfers up into maxTransactionBatchSize chunks and process them individually.  The "posting" of a complete
        // Journal Entry therefore entails posting all of the necessary individually chunked transactions in an all or nothing process.  If a Revert()
        // occurs at any time during the processing of these individual transactions, the entire state is rolled back and no state change will take palce.
        //
        // Determine the number of unique transactions it will take to complete this journal entry's entire "atomic" swap based on 
        // passed in accountTransferCount and maxTransactionBatchSize
        //
        // NOTE:  We substract one from the maxTransactionBatchSize in the following calculations because we 
        //        have to reserve one entry in the transaction for the Treasury account which will be used to offset all the other 
        //        account transfers.  That is, the Treasury is assumed to supply the tokens for all the other account transfers. 
        uint256 lastUniqueTransactionSize = ((accountTransferCount - 1) % (uint256(uint64(transferAmounts[0])) - 1)) + 1;  
        uint256 uniqueTransactionCount = accountTransferCount / (uint256(uint64(transferAmounts[0])) - 1);
        if( uniqueTransactionCount * (uint256(uint64(transferAmounts[0])) - 1) < accountTransferCount )
        {
            uniqueTransactionCount++;            
        }      
        // Initialize a variable to track which account transfer is being processed at a given loop iteration        
        uint256 currentAccount = 2;  // NOTE:  2 because we skip the first two slots in the  transferAddresses/transferAmounts arrays 
        // Loop through the number of unique transactions we need to create to complete this "atomic" swap
        for( uint256 uniqueTransaction = 0; uniqueTransaction < uniqueTransactionCount; uniqueTransaction++)
        {
            transferAmounts[1] = 0;  // Reset totalTokensToTransferFromTreasury for each unique transaction
            // Determine the size of this unique transaction
            uint256 currentUniqueTransactionSize = (
                    uniqueTransaction == uniqueTransactionCount - 1 ?  
                        lastUniqueTransactionSize + 1 :      // Partial unique Transaction (i.e.; lastUniqueTransactionSize in size) - +1 for the offsetting Treasury account
                        uint256(uint64(transferAmounts[0]))  // Fullsize unique Transaction (i.e.; maxTransactionBatchSize in size)            
                                                    );
            // Allocate the parallel addresses and amounts arrays needed for this unique transaction
            address[] memory addresses = new address[](currentUniqueTransactionSize);
            int64[] memory amounts = new int64[](currentUniqueTransactionSize);
            // Loop to initialize above parallel arrays and total the number of treasury tokens we need to offset the total account amounts in the unique transaction
            uint256 accountAddressAmountPair;
            for( accountAddressAmountPair = 0; accountAddressAmountPair < (currentUniqueTransactionSize - 1); accountAddressAmountPair++)
            {   
                amounts[accountAddressAmountPair]   = transferAmounts[currentAccount] * (isReversing == 1 ? int64(-1) : int64(1));   // *-1 if we are reversing the entry            
                addresses[accountAddressAmountPair] = transferAddresses[currentAccount]; 
                transferAmounts[1] +=  transferAmounts[currentAccount];
                currentAccount++;    
            }
            // Now add the required extra offseting Treasury account address/amount in the final slot of the parallel arrays allocated above
            addresses[accountAddressAmountPair]  = transferAddresses[1];                             // Treasury Id
            amounts[accountAddressAmountPair]    = transferAmounts[1] * ( isReversing == 1 ? int64(1): int64(-1)); // totalTokensToTransferFromTreasury - * -1 as tokens are subtracted from Treasury 
            // Perform atomic swap for this unique transaction only
            (bool success, bytes memory result) = precompileAddress.call(
                         abi.encodeWithSelector(IHederaTokenService.transferTokens.selector,
                         transferAddresses[0], addresses, amounts));
            
            if( (success ? abi.decode(result, (int32)) : HederaResponseCodes.UNKNOWN) != HederaResponseCodes.SUCCESS )
            {
                if(uniqueTransaction == 0)  // Were we attempting the very first unique transaction in this batch?
                {
                    // NOTE: we do not need to Revert as the above call returned false and therefore no state has been changed to this point in the contract executiion
                    //       so we can simply return the actual reason for the failure to the caller.
                    return int( abi.decode(result, (int32)) );  
                }
                else
                {
                    revert("Transfer Failed");        // NOTE:  We revert here to roll back any previously successful unique transactions, ensure no state change in overall transaction 
                }
            }
        } 
        // If we make it here we have successfully posted the entire journal entry as an atomic step (potentiall using multiple individual calls to HTS) 
        // so return success
        return HederaResponseCodes.SUCCESS; 
	}

    function toAddress(bytes memory _bytes, uint256 _start) internal pure returns (address) {
        require(_bytes.length >= _start + 20, "toAddress_outOfBounds");
        address tempAddress;

        assembly {
            tempAddress := div(mload(add(add(_bytes, 0x20), _start)), 0x1000000000000000000000000)
        }
        return tempAddress;
    }

    function toInt64(bytes memory _bytes, uint256 _start) internal pure returns (int64) {
        require(_bytes.length >= _start + 8, "toUint64_outOfBounds");
        int64 tempint;

        assembly {
            tempint := mload(add(add(_bytes, 0x8), _start))
        }
        return tempint;
    }
}