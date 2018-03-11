using System.Threading.Tasks;

namespace DeployMyContract.Wpf.Logic
{
    public interface ITabNavigator
    {
        Task GoBackAsync();
        Task GoForwardAsync();

        bool CanGoBack();
        bool CanGoForward();
    }
}
