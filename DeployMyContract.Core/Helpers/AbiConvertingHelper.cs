using System.Linq;
using DeployMyContract.Core.Data;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;

namespace DeployMyContract.Core.Helpers
{
    public static class AbiConvertingHelper
    {
        public static byte[] ConvertConstructorArguments(ConstructorArgument[] arguments)
            => new ConstructorCallEncoder()
                .EncodeParameters(arguments
                        .Select((x, i) => new Parameter(x.Type, x.Name, i + 1))
                        .ToArray(),
                    arguments.Select(x => x.ParsedValue).ToArray());
    }
}
