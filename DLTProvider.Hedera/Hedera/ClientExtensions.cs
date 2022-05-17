using Hashgraph;
using System.Diagnostics;

namespace DLTProviders.Hedera.Hedera;

public static class CliientExtensions
{
    public static async Task<TRecord> RetryKnownNetworkIssues<TRecord>(this Client client, Func<Client, Task<TRecord>> callback) where TRecord : TransactionRecord
    {
        try
        {
            while (true)
            {
                try
                {
                    return await callback(client).ConfigureAwait(false);
                }
                catch (PrecheckException pex) when (pex.Status == ResponseCode.TransactionExpired || pex.Status == ResponseCode.Busy)
                {
                    continue;
                }
            }
        }
        catch (TransactionException ex) when (ex.Message?.StartsWith("The Network Changed the price of Retrieving a Record while attempting to retrieve this record") == true)
        {
            var record = await client.GetTransactionRecordAsync(ex.TxId) as TRecord;
            if (record is not null)
            {
                return record;
            }
            else
            {
                throw;
            }
        }
    }

    public static async Task<TReceipt> RetryKnownNetworkIssuesForReceipt<TReceipt>(this Client client, Func<Client, Task<TReceipt>> callback) where TReceipt : TransactionReceipt
    {
        try
        {
            while (true)
            {
                try
                {
                    return await callback(client).ConfigureAwait(false);
                }
                catch (PrecheckException pex) when (pex.Status == ResponseCode.TransactionExpired || pex.Status == ResponseCode.Busy)
                {
                    Debug.Print($"PRECHECK:  {pex}");
                    await Task.Yield();
                    continue;
                }
                catch( Exception ex)
                {
                    Debug.Print($"EXCEPTION: {ex}");
                    await Task.Yield();
                    continue;
                }
            }
        }
        catch (Exception ex) //when (ex.Message?.StartsWith("The Network Changed the price of Retrieving a Record while attempting to retrieve this record") == true)
        {
            Debug.Print($"Error in RetryKnownNetworkIssuesForReceipt() - {ex}");
            throw;
            //var record = await client.GetTransactionRecordAsync(ex.TxId) as TRecord;
            //if (record is not null)
            //{
            //    return record;
            //}
            //else
            //{
            //    throw;
            //}
        }
    }


}