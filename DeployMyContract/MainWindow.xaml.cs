using System.Windows;
using System.Windows.Input;
using DeployMyContract.Wpf.Data;
using DeployMyContract.Wpf.Logic;

namespace DeployMyContract.Wpf
{
    public partial class MainWindow
    {
        #region dp TabNavigator { get; }
        public static readonly DependencyProperty TabNavigatorProperty = DependencyProperty.Register(
            "TabNavigator", typeof(ITabNavigator), typeof(MainWindow), new PropertyMetadata(default(ITabNavigator)));
        public ITabNavigator TabNavigator
        {
            get => (ITabNavigator) GetValue(TabNavigatorProperty);
            private set => SetValue(TabNavigatorProperty, value);
        }
        #endregion

        #region dp PageContext { get; }
        public static readonly DependencyProperty PageContextProperty = DependencyProperty.Register(
            "PageContext", typeof(PageContext), typeof(MainWindow), new PropertyMetadata(default(PageContext)));
        public PageContext PageContext
        {
            get => (PageContext) GetValue(PageContextProperty);
            private set => SetValue(PageContextProperty, value);
        }
        #endregion

        public MainWindow()
        {
            PageContext = new PageContext();

            InitializeComponent();

            TabNavigator = new TabNavigator(MainTabControl);

            CommandBindings.Add(new CommandBinding(
                NavigationCommands.BrowseBack,
                (s, e) => TabNavigator.GoBackAsync(),
                (s, e) => e.CanExecute = TabNavigator.CanGoBack()));
            CommandBindings.Add(new CommandBinding(
                NavigationCommands.BrowseForward,
                (s, e) => TabNavigator.GoForwardAsync(),
                (s, e) => e.CanExecute = TabNavigator.CanGoForward()));
        }
    }
}
