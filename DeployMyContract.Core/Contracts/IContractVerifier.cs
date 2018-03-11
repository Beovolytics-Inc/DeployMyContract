using System.Threading.Tasks;
using DeployMyContract.Core.Data;

namespace DeployMyContract.Core.Contracts
{
    public interface IContractVerifier
    {
        Task<string[]> GetSupportedCompilersAsync();
        Task VerifyAsync(VerifyRequest request);
    }
}