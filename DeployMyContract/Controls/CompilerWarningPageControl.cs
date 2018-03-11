using DeployMyContract.Wpf.Data;

namespace DeployMyContract.Wpf.Controls
{
    public class CompilerWarningPageControl : PageControlBase
    {
        protected override PageIndex? PlannedNextPageIndex => PageIndex.Deployment;
        protected override PageIndex? PlannedPreviousPageIndex => PageIndex.ContractOptions;
    }
}
