using System;
using System.Numerics;
using System.Threading.Tasks;
using DeployMyContract.Core.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.TransactionReceipts;
using Nethereum.Web3;
using Newtonsoft.Json;

namespace DeployMyContract.Core.Logic
{
    public class EthereumAccount : IEthereumAccount
    {
        private const double WeisInEth = 1e18;
        private const int TransferGasAmount = 21000;

        private readonly Web3 m_Web3;
        private readonly string m_WalletAddress;
        private readonly Uri m_EtherscanUri;

        public EthereumAccount(Web3 web3, string walletAddress, Uri etherscanUri)
        {
            m_Web3 = web3 ?? throw new ArgumentNullException(nameof(web3));
            m_WalletAddress = walletAddress ?? throw new ArgumentNullException(nameof(walletAddress));
            m_EtherscanUri = etherscanUri ?? throw new ArgumentNullException(nameof(etherscanUri));
        }

        public async Task<double> GetBalanceAsync()
        {
            string response;
            using (var client = HttpClientFactory.CreateFirefox(m_EtherscanUri))
            using (var getResponse = await client.GetAsync(
                new Uri(m_EtherscanUri, $"/api?module=account&action=balance&address={m_WalletAddress}&tag=latest")))
                response = await getResponse.Content.ReadAsStringAsync();

            dynamic responseJson = JsonConvert.DeserializeObject(response);
            if ((int)responseJson.status != 1)
                return 0;
            return (double)responseJson.result / WeisInEth;
        }

        public async Task<double> GetMinimalBalanceAsync(int deploymentGasLimit, double fee)
        {
            var gasPriceWei = (await m_Web3.Eth.GasPrice.SendRequestAsync()).Value;
            return (double)((deploymentGasLimit + TransferGasAmount) * gasPriceWei) / WeisInEth + fee;
        }

        public async Task<string> SendEthAsync(string target, double amount)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var tx = await m_Web3.Eth.TransactionManager.SendTransactionAsync(
                m_WalletAddress, target, new HexBigInteger((BigInteger)(amount * WeisInEth)));
            var txPollingService = new TransactionReceiptPollingService(m_Web3.TransactionManager);
            await txPollingService.PollForReceiptAsync(tx);
            return tx;
        }
    }
}
