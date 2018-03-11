using System;
using System.Linq;
using System.Threading.Tasks;
using DeployMyContract.Core.Contracts;
using DeployMyContract.Core.Data;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionReceipts;
using Nethereum.Web3;

namespace DeployMyContract.Core.Logic
{
    public class ContractDeployer : IContractDeployer
    {
        protected WalletInfo Owner { get; }

        private readonly Web3 m_Web3;
        private readonly ITransactionReceiptService m_ReceiptService;

        public ContractDeployer(Web3 web3, ITransactionReceiptService receiptService, WalletInfo owner)
        {
            m_Web3 = web3 ?? throw new ArgumentNullException(nameof(web3));
            m_ReceiptService = receiptService ?? throw new ArgumentNullException(nameof(receiptService));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public async Task<string> StartDeployingAsync(DeployRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await m_Web3.Eth.DeployContract.SendRequestAsync(
                request.Abi, request.Bin, Owner.Address, new HexBigInteger(request.GasLimit),
                request.ConstructorArguments
                    .Select(x => x.ParsedValue)
                    .ToArray());
        }

        public async Task<string> GetDeployedContractAddressAsync(string transactionHash)
            => (await GetTransactionReceiptAsync(transactionHash))?.ContractAddress;

        public async Task<bool> GetTransactionConfirmationStatusAsync(string transactionHash)
            => (await GetTransactionReceiptAsync(transactionHash))?.BlockHash != null;

        private async Task<TransactionReceipt> GetTransactionReceiptAsync(string transactionHash)
            => await m_ReceiptService.SendRequestAsync(() => Task.FromResult(transactionHash));
    }
}
