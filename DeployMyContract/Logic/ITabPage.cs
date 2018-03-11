using System.Threading.Tasks;
using DeployMyContract.Wpf.Data;

namespace DeployMyContract.Wpf.Logic
{
    public interface ITabPage
    {
        bool IsValid { get; }
        PageIndex? NextPageIndex { get; }
        PageIndex? PreviousPageIndex { get; }

        Task OnLoadedAsync();
        Task<bool> OnBackAsync();
        Task<bool> OnForwardAsync();
    }
}
