using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeployMyContract.Wpf.Data;
using DeployMyContract.Wpf.Logic;

namespace DeployMyContract.Wpf.Controls
{
    public abstract class PageControlBase : Control, ITabPage
    {
        #region dp PageContext { get; set; }
        public static readonly DependencyProperty PageContextProperty = DependencyProperty.Register(
            "PageContext", typeof(PageContext), typeof(PageControlBase), new PropertyMetadata(default(PageContext)));
        public PageContext PageContext
        {
            get => (PageContext) GetValue(PageContextProperty);
            set => SetValue(PageContextProperty, value);
        }
        #endregion

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            "IsBusy", typeof(bool), typeof(PageControlBase), new PropertyMetadata(default(bool)));
        public bool IsBusy
        {
            get => (bool) GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public virtual bool IsValid => true;

        public virtual PageIndex? NextPageIndex => PageContext?.ErrorMessage != null ? PageIndex.Error : PlannedNextPageIndex;
        public PageIndex? PreviousPageIndex => PageContext?.ReturnPageIndex ?? PlannedPreviousPageIndex;

        protected abstract PageIndex? PlannedNextPageIndex { get; }
        protected abstract PageIndex? PlannedPreviousPageIndex { get; }

        public virtual Task OnLoadedAsync() => Task.FromResult((object)null);
        public virtual Task<bool> OnBackAsync() => Task.FromResult(true);
        public virtual Task<bool> OnForwardAsync() => Task.FromResult(true);

        protected IDisposable ShowThrobber()
        {
            IsBusy = true;
            return new FuncDisposable(() => IsBusy = false);
        }

        protected PageIndex? GetCurrentPageIndex()
        {
            DependencyObject control = this;
            while ((control = VisualTreeHelper.GetParent(control)) != null)
                if (control is TabControl tabControl)
                    return (PageIndex) tabControl.SelectedIndex;
            return null;
        }

        protected MessageBoxResult ShowMessageBox(
            string text, string caption = null, MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.None)
        {
            var parentWindow = Window.GetWindow(this);
            return parentWindow != null
                ? MessageBox.Show(parentWindow, text, caption, buttons, image)
                : MessageBox.Show(text, caption, buttons, image);
        }

        protected void SetError(string error)
        {
            PageContext.ErrorMessage = error;
            PageContext.ReturnPageIndex = GetCurrentPageIndex();
        }

        private class FuncDisposable : IDisposable
        {
            private readonly Action m_OnDispose;
            private bool m_Disposed;

            public FuncDisposable(Action onDispose)
            {
                m_OnDispose = onDispose;
            }

            public void Dispose()
            {
                if (m_Disposed)
                    return;
                m_OnDispose.Invoke();
                m_Disposed = true;
            }
        }
    }
}
