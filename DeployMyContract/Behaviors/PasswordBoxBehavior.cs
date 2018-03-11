using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DeployMyContract.Wpf.Behaviors
{
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register(
            "Binding", typeof(string), typeof(PasswordBoxBehavior), new PropertyMetadata(default(string)));
        public string Binding
        {
            get => (string) GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.PasswordChanged += HandlePasswordChanged;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= HandlePasswordChanged;
        }

        private void HandlePasswordChanged(object sender, RoutedEventArgs e) 
            => Binding = AssociatedObject.Password;
    }
}
