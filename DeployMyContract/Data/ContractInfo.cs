using DeployMyContract.Core.Data;

namespace DeployMyContract.Wpf.Data
{
    public class ContractInfo
    {
        public string Name { get; set; }
        public string Abi { get; set; }
        public string Bin { get; set; }
        public ConstructorArgument[] ConstructorArguments { get; set; }
    }
}
