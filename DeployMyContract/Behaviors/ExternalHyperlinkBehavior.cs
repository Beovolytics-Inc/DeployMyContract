using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Navigation;

namespace DeployMyContract.Wpf.Behaviors
{
    public class ExternalHyperlinkBehavior : Behavior<Hyperlink>
    {
        protected override void OnAttached()
        {
            AssociatedObject.RequestNavigate += HandleRequestNavigate;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RequestNavigate -= HandleRequestNavigate;
        }

        private static void HandleRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
    }
}
