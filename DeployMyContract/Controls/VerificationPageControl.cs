using System;
using System.Threading.Tasks;
using System.Windows;
using DeployMyContract.Core.Data;
using DeployMyContract.Core.Helpers;
using DeployMyContract.Core.Logic;
using DeployMyContract.Wpf.Data;

namespace DeployMyContract.Wpf.Controls
{
    public class VerificationPageControl : PageControlBase
    {
        public static readonly DependencyProperty TransactionUrlProperty = DependencyProperty.Register(
            "TransactionUrl", typeof(string), typeof(VerificationPageControl), new PropertyMetadata(default(string)));
        public string TransactionUrl
        {
            get => (string) GetValue(TransactionUrlProperty);
            set => SetValue(TransactionUrlProperty, value);
        }

        protected override PageIndex? PlannedNextPageIndex => PageIndex.Finish;
        protected override PageIndex? PlannedPreviousPageIndex => PageIndex.Deployment;

        public override Task OnLoadedAsync()
        {
            PageContext.ContractUri = new Uri(PageContext.SelectedEthereum.Url, $"/address/{PageContext.ContractAddress}");
            TransactionUrl = new Uri(PageContext.SelectedEthereum.Url, $"/tx/{PageContext.DeploymentTransaction}").ToString();
            return base.OnLoadedAsync();
        }

        public override async Task<bool> OnForwardAsync()
        {
            try
            {
                using (ShowThrobber())
                {
                    var verifier = new EtherscanContractVerifier(
                        new Uri(PageContext.SelectedEthereum.Url, PageContext.SelectedEthereum.VerificationUrl));
                    await verifier.VerifyAsync(new VerifyRequest
                    {
                        Name = PageContext.SelectedContract.Name,
                        AbiEncodedConstructorArgs = AbiConvertingHelper.ConvertConstructorArguments(
                            PageContext.SelectedContract.ConstructorArguments),
                        Compiler = PageContext.CompilerVersion,
                        ContractAddress = PageContext.ContractAddress,
                        Source = PageContext.ContractSource
                    });
                    await PageContext.CurrentAccount.SendEthAsync(
                        PageContext.SelectedEthereum.FeeTargetAddress,
                        PageContext.SelectedEthereum.Fee);
                }
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }

            return true;
        }
    }
}
