using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace dAccounting.Common.Utilities
{
    public class ResourceUtilities
    {
        public static string? ReadResourceAsString(Assembly? resourceAssembly, string name)
        {
            string? content = null!;
            if (resourceAssembly != null!)
            {
                Stream? stream = null!;
                using (stream = resourceAssembly.GetManifestResourceStream(name))
                {
                    if (stream != null!)
                    {
                        StreamReader? reader = null!;
                        
                            using (reader = new StreamReader(stream))
                            {
                                content = reader.ReadToEnd();
                            }
                        reader = null!;
                    }
                }
                stream = null!;
            }
            return content;
        }
    }
}
