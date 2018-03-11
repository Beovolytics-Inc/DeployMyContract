namespace DeployMyContract.Core.Data
{
    public class CompilationResult
    {
        public string Warnings { get; set; }
        public CompilationOutput[] Outputs { get; set; }
    }
}
