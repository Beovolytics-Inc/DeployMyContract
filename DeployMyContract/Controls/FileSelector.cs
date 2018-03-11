using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeployMyContract.Wpf.Logic;
using Microsoft.Win32;

namespace DeployMyContract.Wpf.Controls
{
    public class FileSelector : Control
    {
        public string Filter { get; set; }

        #region dp FileName { get; set; }
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            "FileName", typeof(string), typeof(FileSelector), new PropertyMetadata(default(string)));
        public string FileName
        {
            get => (string) GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        #endregion

        public FileSelector()
        {
            CommandBindings.Add(
                new CommandBinding(
                    BrowseCommand.Default,
                    (s, e) => HandleBrowseCommand()));
        }

        private void HandleBrowseCommand()
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = Filter
            };
            if (dialog.ShowDialog() != true)
                return;
            FileName = dialog.FileName;
        }
    }
}
