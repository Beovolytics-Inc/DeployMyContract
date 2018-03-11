using System.Threading.Tasks;

namespace DeployMyContract.Core.Contracts
{
    public interface IEthereumAccount
    {
        Task<double> GetBalanceAsync();
        Task<double> GetMinimalBalanceAsync(int deploymentGasLimit, double fee);
        Task<string> SendEthAsync(string target, double amount);
    }
}
