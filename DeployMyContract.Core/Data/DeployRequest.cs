namespace DeployMyContract.Core.Data
{
    public class DeployRequest
    {
        public string Bin { get; set; }
        public string Abi { get; set; }
        public int GasLimit { get; set; }
        public ConstructorArgument[] ConstructorArguments { get; set; }
    }
}
