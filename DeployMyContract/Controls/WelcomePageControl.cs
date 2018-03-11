using DeployMyContract.Wpf.Data;

namespace DeployMyContract.Wpf.Controls
{
    public class WelcomePageControl : PageControlBase
    {
        protected override PageIndex? PlannedNextPageIndex => PageIndex.ContractOptions;
        protected override PageIndex? PlannedPreviousPageIndex => null;
    }
}