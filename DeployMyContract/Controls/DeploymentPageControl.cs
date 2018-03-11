using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DeployMyContract.Core.Data;
using DeployMyContract.Core.Logic;
using DeployMyContract.Wpf.Data;
using DeployMyContract.Wpf.Properties;
using Nethereum.KeyStore.Crypto;
using Nethereum.RPC.TransactionReceipts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace DeployMyContract.Wpf.Controls
{
    public class DeploymentPageControl : PageControlBase
    {
        public static readonly DependencyProperty KeyPathProperty = DependencyProperty.Register(
            "KeyPath", typeof(string), typeof(DeploymentPageControl), new PropertyMetadata(default(string)));
        public string KeyPath
        {
            get => (string) GetValue(KeyPathProperty);
            set => SetValue(KeyPathProperty, value);
        }

        public static readonly DependencyProperty KeyPasswordProperty = DependencyProperty.Register(
            "KeyPassword", typeof(string), typeof(DeploymentPageControl), new PropertyMetadata(default(string)));
        public string KeyPassword
        {
            get => (string) GetValue(KeyPasswordProperty);
            set => SetValue(KeyPasswordProperty, value);
        }

        public static readonly DependencyProperty GasLimitProperty = DependencyProperty.Register(
            "GasLimit", typeof(int), typeof(DeploymentPageControl), new PropertyMetadata(Settings.Default.DefaultDeploymentGasLimit));
        public int GasLimit
        {
            get => (int) GetValue(GasLimitProperty);
            set => SetValue(GasLimitProperty, value);
        }

        public override bool IsValid
            => !string.IsNullOrWhiteSpace(KeyPath)
                && !string.IsNullOrWhiteSpace(PageContext.EthereumNodeUrl);

        protected override PageIndex? PlannedNextPageIndex => PageIndex.Verification;
        protected override PageIndex? PlannedPreviousPageIndex => PageIndex.ContractOptions;

        public override Task OnLoadedAsync()
        {
            if (PageContext.SelectedContract == null)
                PageContext.SelectedContract = PageContext.CompiledContractInfos.FirstOrDefault();
            return base.OnLoadedAsync();
        }

        public override async Task<bool> OnForwardAsync()
        {
            if (!Uri.IsWellFormedUriString(PageContext.EthereumNodeUrl, UriKind.Absolute))
            {
                ShowMessageBox("Invalid Ethereum node URL", image: MessageBoxImage.Exclamation);
                return false;
            }
            var keyFile = new FileInfo(KeyPath);
            if (!keyFile.Exists)
            {
                ShowMessageBox("Specified key file doesn't exist", image: MessageBoxImage.Exclamation);
                return false;
            }
            try
            {
                var argumentValueParser = new ArgumentValueParser();
                foreach (var argument in PageContext.SelectedContract.ConstructorArguments)
                    argument.ParsedValue = argumentValueParser.Parse(argument.Name, argument.Type, argument.Value);

                var keyJson = File.ReadAllText(keyFile.FullName);
                using (ShowThrobber())
                    await DeployContract(
                        new Uri(PageContext.EthereumNodeUrl),
                        new WalletInfo(keyJson, KeyPassword ?? string.Empty),
                        PageContext.SelectedContract);
            }
            catch (DecryptionException)
            {
                SetError("Couldn't decrypt the keystore. Probably the password is invalid.");
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }

            return true;
        }

        private async Task DeployContract(Uri nodeUri, WalletInfo deployerWallet, ContractInfo contract)
        {
            var web3 = new Web3(new Account(deployerWallet.PrivateKey), nodeUri.ToString());
            PageContext.CurrentAccount = new EthereumAccount(web3, deployerWallet.Address, 
                PageContext.SelectedEthereum.ApiUrl ?? PageContext.SelectedEthereum.Url);
            var accountBalance = await PageContext.CurrentAccount.GetBalanceAsync();
            if (accountBalance <= 0)
                throw new ContractDeploymentException(
                    $"Balance of your wallet {deployerWallet.Address} is 0. "
                    + "It may mean that your wallet doesn't exist in current Ethereum network. Check that you've chosen the correct one.");
            var minBalance = await PageContext.CurrentAccount.GetMinimalBalanceAsync(GasLimit, PageContext.SelectedEthereum.Fee);
            if (accountBalance < minBalance)
                throw new ContractDeploymentException(
                    $"Balance of your wallet {deployerWallet.Address} ({accountBalance:N6}) is insufficient. "
                    + "To deploy the contract on the selected network with specified gas limit," 
                    + $" the balance of your wallet should be greater than {minBalance:N6} ETH");

            var deployer = new ContractDeployer(
                web3,
                new TransactionReceiptPollingService(web3.TransactionManager),
                deployerWallet);
            var transaction = await deployer.StartDeployingAsync(new DeployRequest
            {
                Abi = contract.Abi,
                Bin = contract.Bin,
                ConstructorArguments = contract.ConstructorArguments,
                GasLimit = GasLimit
            });
            PageContext.DeploymentTransaction = transaction;
            PageContext.ContractAddress = await deployer.GetDeployedContractAddressAsync(transaction);
        }
    }
}
