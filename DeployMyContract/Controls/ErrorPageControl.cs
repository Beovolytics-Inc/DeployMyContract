using System.Threading.Tasks;
using DeployMyContract.Wpf.Data;

namespace DeployMyContract.Wpf.Controls
{
    public class ErrorPageControl : PageControlBase
    {
        public override PageIndex? NextPageIndex => null;
        protected override PageIndex? PlannedNextPageIndex => null;
        protected override PageIndex? PlannedPreviousPageIndex => m_ReturnPage;

        private PageIndex? m_ReturnPage;

        public override Task<bool> OnBackAsync()
        {
            m_ReturnPage = PageContext.ReturnPageIndex;
            PageContext.ReturnPageIndex = null;
            PageContext.ErrorMessage = null;
            return Task.FromResult(true);
        }
    }
}
