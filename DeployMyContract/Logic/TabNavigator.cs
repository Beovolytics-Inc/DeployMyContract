using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DeployMyContract.Wpf.Data;

namespace DeployMyContract.Wpf.Logic
{
    public class TabNavigator : ITabNavigator
    {
        private readonly TabControl m_TabControl;

        public TabNavigator(TabControl tabControl)
        {
            m_TabControl = tabControl ?? throw new ArgumentNullException(nameof(tabControl));
            m_TabControl.SelectedIndex = (int) PageIndex.Welcome;
        }

        public async Task GoBackAsync()
        {
            if (await GetCurrentPage().OnBackAsync())
                await NavigateToPage(GetCurrentPage().PreviousPageIndex.GetValueOrDefault());
        }

        public async Task GoForwardAsync()
        {
            if (await GetCurrentPage().OnForwardAsync())
                await NavigateToPage(GetCurrentPage().NextPageIndex.GetValueOrDefault());
        }

        public bool CanGoBack()
            => GetCurrentPage()?.PreviousPageIndex != null;

        public bool CanGoForward()
            => GetCurrentPage()?.NextPageIndex != null && GetCurrentPage().IsValid;

        private async Task NavigateToPage(PageIndex pageIndex)
        {
            m_TabControl.SelectedIndex = (int) pageIndex;
            CommandManager.InvalidateRequerySuggested();
            await GetCurrentPage().OnLoadedAsync();
        }

        private ITabPage GetCurrentPage()
            => m_TabControl.SelectedContent as ITabPage;
    }
}
