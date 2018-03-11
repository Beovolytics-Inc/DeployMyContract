using System;
using System.IO;
using System.Windows;
using DeployMyContract.Core.Contracts;
using DeployMyContract.Core.Data;
using DeployMyContract.Wpf.Properties;

namespace DeployMyContract.Wpf.Data
{
    public class PageContext : DependencyObject
    {
        #region dp CompilerPath { get; }
        public static readonly DependencyProperty CompilerPathProperty = DependencyProperty.Register(
            "CompilerPath", typeof(string), typeof(PageContext), 
            new PropertyMetadata(Path.Combine(Directory.GetCurrentDirectory(), "solc.exe")));
        public string CompilerPath
        {
            get => (string) GetValue(CompilerPathProperty);
            set => SetValue(CompilerPathProperty, value);
        }
        #endregion

        #region dp ContractSource { get; }
        public static readonly DependencyProperty ContractSourceProperty = DependencyProperty.Register(
            "ContractSource", typeof(string), typeof(PageContext), new PropertyMetadata(default(string)));
        public string ContractSource
        {
            get => (string) GetValue(ContractSourceProperty);
            set => SetValue(ContractSourceProperty, value);
        }
        #endregion

        public static readonly DependencyProperty EthereumNodeUrlProperty = DependencyProperty.Register(
            "EthereumNodeUrl", typeof(string), typeof(PageContext), new PropertyMetadata(Settings.Default.EthereumNodeUrl));
        public string EthereumNodeUrl
        {
            get => (string) GetValue(EthereumNodeUrlProperty);
            set => SetValue(EthereumNodeUrlProperty, value);
        }

        public static readonly DependencyProperty SelectedContractProperty = DependencyProperty.Register(
            "SelectedContract", typeof(ContractInfo), typeof(PageContext), new PropertyMetadata(default(ContractInfo)));
        public ContractInfo SelectedContract
        {
            get => (ContractInfo) GetValue(SelectedContractProperty);
            set => SetValue(SelectedContractProperty, value);
        }

        public static readonly DependencyProperty CompilerVersionProperty = DependencyProperty.Register(
            "CompilerVersion", typeof(string), typeof(PageContext), new PropertyMetadata(default(string)));
        public string CompilerVersion
        {
            get => (string) GetValue(CompilerVersionProperty);
            set => SetValue(CompilerVersionProperty, value);
        }

        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            "ErrorMessage", typeof(string), typeof(PageContext), new PropertyMetadata(default(string)));
        public string ErrorMessage
        {
            get => (string) GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        public static readonly DependencyProperty ContractAddressProperty = DependencyProperty.Register(
            "ContractAddress", typeof(string), typeof(PageContext), new PropertyMetadata(default(string)));
        public string ContractAddress
        {
            get => (string) GetValue(ContractAddressProperty);
            set => SetValue(ContractAddressProperty, value);
        }

        public static readonly DependencyProperty DeploymentTransactionProperty = DependencyProperty.Register(
            "DeploymentTransaction", typeof(string), typeof(PageContext), new PropertyMetadata(default(string)));
        public string DeploymentTransaction
        {
            get => (string) GetValue(DeploymentTransactionProperty);
            set => SetValue(DeploymentTransactionProperty, value);
        }

        public static readonly DependencyProperty CompiledContractInfosProperty = DependencyProperty.Register(
            "CompiledContractInfos", typeof(ContractInfo[]), typeof(PageContext), new PropertyMetadata(default(ContractInfo[])));
        public ContractInfo[] CompiledContractInfos
        {
            get => (ContractInfo[]) GetValue(CompiledContractInfosProperty);
            set => SetValue(CompiledContractInfosProperty, value);
        }

        public static readonly DependencyProperty SelectedEthereumProperty = DependencyProperty.Register(
            "SelectedEthereum", typeof(EthereumNetworkInfo), typeof(PageContext), new PropertyMetadata(default(EthereumNetworkInfo)));
        public EthereumNetworkInfo SelectedEthereum
        {
            get => (EthereumNetworkInfo) GetValue(SelectedEthereumProperty);
            set => SetValue(SelectedEthereumProperty, value);
        }

        public static readonly DependencyProperty ContractUriProperty = DependencyProperty.Register(
            "ContractUri", typeof(Uri), typeof(PageContext), new PropertyMetadata(default(Uri)));
        public Uri ContractUri
        {
            get => (Uri) GetValue(ContractUriProperty);
            set => SetValue(ContractUriProperty, value);
        }

        public static readonly DependencyProperty CompilerWarningsProperty = DependencyProperty.Register(
            "CompilerWarnings", typeof(string), typeof(PageContext), new PropertyMetadata(default(string)));
        public string CompilerWarnings
        {
            get => (string) GetValue(CompilerWarningsProperty);
            set => SetValue(CompilerWarningsProperty, value);
        }

        public PageIndex? ReturnPageIndex { get; set; }

        public IEthereumAccount CurrentAccount { get; set; }
    }
}
