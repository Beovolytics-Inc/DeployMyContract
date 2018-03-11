using System.Windows.Input;

namespace DeployMyContract.Wpf.Logic
{
    public class BrowseCommand
    {
        public static ICommand Default { get; } = new RoutedUICommand("Browse", "Browse", typeof(BrowseCommand));

        private BrowseCommand()
        { }
    }
}
