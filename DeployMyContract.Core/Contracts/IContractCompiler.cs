using System.Threading.Tasks;
using DeployMyContract.Core.Data;

namespace DeployMyContract.Core.Contracts
{
    public interface IContractCompiler
    {
        Task<CompilerVersion> GetVersionAsync();
        Task<CompilationResult> CompileAsync(string source);
    }
}
