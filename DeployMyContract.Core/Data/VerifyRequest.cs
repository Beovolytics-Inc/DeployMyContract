namespace DeployMyContract.Core.Data
{
    public class VerifyRequest
    {
        public string Name { get; set; }
        public string Compiler { get; set; }
        public string Source { get; set; }
        public string ContractAddress { get; set; }
        public byte[] AbiEncodedConstructorArgs { get; set; }
    }
}
