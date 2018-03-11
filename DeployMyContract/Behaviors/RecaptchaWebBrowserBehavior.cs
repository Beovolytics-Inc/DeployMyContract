using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DeployMyContract.Wpf.Behaviors
{
    public class RecaptchaWebBrowserBehavior : Behavior<WebBrowser>
    {
        protected override void OnAttached()
        {
            AssociatedObject.NavigateToString(@"
<html>
<head>
    <script>
        delete window.location;
    </script>
    <script src='https://www.google.com/recaptcha/api.js' async defer></script>
</head>
<body>
    <div id='recaptcha-demo' class='g-recaptcha' data-sitekey='6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-' />
</body>
</html>");
        }
    }
}
