using System.Threading.Tasks;
using DeployMyContract.Core.Data;

namespace DeployMyContract.Core.Contracts
{
    public interface IContractDeployer
    {
        Task<string> StartDeployingAsync(DeployRequest request);
        Task<string> GetDeployedContractAddressAsync(string transactionHash);
        Task<bool> GetTransactionConfirmationStatusAsync(string transactionHash);
    }
}
