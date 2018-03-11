using System.Collections.Generic;

namespace DeployMyContract.Core.Data
{
    public class CompilationOutput
    {
        public string ContractName { get; set; }
        public IReadOnlyDictionary<string, string> ConstructorParameterTypes { get; set; }
        public string Bin { get; set; }
        public string Abi { get; set; }
    }
}
