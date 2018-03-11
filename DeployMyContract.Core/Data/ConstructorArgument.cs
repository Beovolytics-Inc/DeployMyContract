namespace DeployMyContract.Core.Data
{
    public class ConstructorArgument
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public object ParsedValue { get; set; }
    }
}