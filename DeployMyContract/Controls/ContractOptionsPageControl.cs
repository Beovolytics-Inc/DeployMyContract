using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DeployMyContract.Core.Data;
using DeployMyContract.Core.Logic;
using DeployMyContract.Wpf.Data;

namespace DeployMyContract.Wpf.Controls
{
    public class ContractOptionsPageControl : PageControlBase
    {
        public static readonly DependencyProperty ContractSourcePathProperty = DependencyProperty.Register(
            "ContractSourcePath", typeof(string), typeof(ContractOptionsPageControl), new PropertyMetadata(default(string)));
        public string ContractSourcePath
        {
            get => (string) GetValue(ContractSourcePathProperty);
            set => SetValue(ContractSourcePathProperty, value);
        }

        public static readonly DependencyProperty EthereumNetworkInfosProperty = DependencyProperty.Register(
            "EthereumNetworkInfos", typeof(EthereumNetworkInfo[]), typeof(ContractOptionsPageControl), new PropertyMetadata(default(EthereumNetworkInfo[])));
        public EthereumNetworkInfo[] EthereumNetworkInfos
        {
            get => (EthereumNetworkInfo[]) GetValue(EthereumNetworkInfosProperty);
            private set => SetValue(EthereumNetworkInfosProperty, value);
        }

        public override bool IsValid
            => !string.IsNullOrWhiteSpace(PageContext.CompilerPath)
               && (!string.IsNullOrWhiteSpace(PageContext.ContractSource)
                   || !string.IsNullOrWhiteSpace(ContractSourcePath));

        protected override PageIndex? PlannedNextPageIndex
            => string.IsNullOrWhiteSpace(PageContext.CompilerWarnings) ? PageIndex.Deployment : PageIndex.CompilerWarning;
        protected override PageIndex? PlannedPreviousPageIndex => PageIndex.Welcome;

        public ContractOptionsPageControl()
        {
            EthereumNetworkInfos = new[]
            {
                new EthereumNetworkInfo
                {
                    Name = "MainNet",
                    Url = new Uri("https://etherscan.io/"),
                    ApiUrl = new Uri("https://api.etherscan.io/"),
                    VerificationUrl = "/verifyContract",
                    Fee = 0.05,
                    FeeTargetAddress = "0x3161defcbd681ff0352d3526a6235d826f274f94"
                },
                new EthereumNetworkInfo
                {
                    Name = "Kovan test network",
                    Url = new Uri("https://kovan.etherscan.io"),
                    VerificationUrl = "/verifyContract",
                    Fee = 0.0007,
                    FeeTargetAddress = "0x170e655b43275c2ea664cd18014c22cf1547959c"
                },
                new EthereumNetworkInfo
                {
                    Name = "Ropsten test network",
                    Url = new Uri("https://ropsten.etherscan.io/"),
                    VerificationUrl = "/verifyContract",
                    Fee = 0.0008,
                    FeeTargetAddress = "0x4453f462aefefc31c06f58d0e9c438534ac8e188"
                },
                new EthereumNetworkInfo
                {
                    Name = "Rinkeby test network",
                    Url = new Uri("https://rinkeby.etherscan.io/"),
                    VerificationUrl = "/verifyContract",
                    Fee = 0.001,
                    FeeTargetAddress = "0x2f3241f91b4e1de7367373ef95825e9bfcf7e58f"
                }
            };
        }

        public override Task OnLoadedAsync()
        {
            if (PageContext.SelectedEthereum == null)
                PageContext.SelectedEthereum = EthereumNetworkInfos.First();
            if (!string.IsNullOrEmpty(ContractSourcePath))
                PageContext.ContractSource = null;
            PageContext.CompilerWarnings = null;
            return base.OnLoadedAsync();
        }

        public override async Task<bool> OnForwardAsync()
        {
            var compilerFile = new FileInfo(PageContext.CompilerPath);
            if (!compilerFile.Exists)
            {
                ShowMessageBox("Solidity compiler wasn't found at the specified path",
                    image: MessageBoxImage.Exclamation);
                return false;
            }
            if (!".exe".Equals(compilerFile.Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                ShowMessageBox("Invalid path to the Solidity compiler",
                    image: MessageBoxImage.Exclamation);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(ContractSourcePath)
                && !File.Exists(ContractSourcePath))
            {
                ShowMessageBox("Source file doesn't exist", image: MessageBoxImage.Exclamation);
                return false;
            }
            var source = PageContext.ContractSource;
            if (!string.IsNullOrWhiteSpace(ContractSourcePath))
            {
                try
                {
                    source = File.ReadAllText(ContractSourcePath);
                }
                catch (Exception ex)
                {
                    ShowMessageBox("Couldn't read source file: " + ex.Message,
                        image: MessageBoxImage.Error);
                    return false;
                }
            }

            PageContext.ContractSource = source;
            try
            {
                using (ShowThrobber())
                    await CompileSource(source, string.IsNullOrWhiteSpace(ContractSourcePath)
                            ? new DirectoryInfo(Directory.GetCurrentDirectory())
                            : new DirectoryInfo(
                                Path.GetDirectoryName(ContractSourcePath) ?? Directory.GetCurrentDirectory()),
                        compilerFile);
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }

            return true;
        }

        private async Task CompileSource(string source, DirectoryInfo currentDirectory, FileInfo compilerFile)
        {
            var parser = new ContractSourceParser();
            var parseResult = parser.Parse(source, currentDirectory);

            var compiler = new ContractCompiler(compilerFile.FullName);
            var compilerVersion = await compiler.GetVersionAsync();

            var verifier = new EtherscanContractVerifier(
                new Uri(PageContext.SelectedEthereum.Url, PageContext.SelectedEthereum.VerificationUrl));
            var supportedVersions = await verifier.GetSupportedCompilersAsync();
            if (!supportedVersions.Contains(compilerVersion.FullVersionString, StringComparer.CurrentCultureIgnoreCase))
                throw new ContractCompilationException(
                    $"Your compiler version {compilerVersion.FullVersionString} isn't supported by {PageContext.SelectedEthereum.Name} contract verifier");

            if (parseResult.PragmaVersion < new Version(compilerVersion.Version.Major, compilerVersion.Version.Minor)
                || parseResult.PragmaVersion >= new Version(compilerVersion.Version.Major, compilerVersion.Version.Minor + 1))
                throw new ContractCompilationException(
                    $"Incompatible version of the compiler: contract requires {parseResult.PragmaVersion}, current version is {compilerVersion.Version}");

            var compilationResult = await compiler.CompileAsync(parseResult.SourceCode);
            if (!compilationResult.Outputs.Any())
                throw new ContractCompilationException("Source file doesn't contain valid Solidity code");

            PageContext.ContractSource = parseResult.SourceCode;
            PageContext.CompilerVersion = compilerVersion.FullVersionString;
            PageContext.CompilerWarnings = compilationResult.Warnings;
            PageContext.CompiledContractInfos = compilationResult.Outputs
                .Select(x => new ContractInfo
                {
                    Name = x.ContractName,
                    Bin = x.Bin,
                    Abi = x.Abi,
                    ConstructorArguments = x.ConstructorParameterTypes
                        .Select(y => new ConstructorArgument
                        {
                            Name = y.Key,
                            Type = y.Value
                        })
                        .ToArray()
                })
                .ToArray();
        }
    }
}
