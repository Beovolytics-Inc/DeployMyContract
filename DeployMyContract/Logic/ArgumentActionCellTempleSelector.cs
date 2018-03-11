using System;
using System.Windows;
using System.Windows.Controls;
using DeployMyContract.Core.Data;

namespace DeployMyContract.Wpf.Logic
{
    public class ArgumentActionCellTempleSelector : DataTemplateSelector
    {
        public DataTemplate IntTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is ConstructorArgument argument))
                return new DataTemplate();
            if (argument.Type.StartsWith("int", StringComparison.InvariantCultureIgnoreCase)
                || argument.Type.StartsWith("uint", StringComparison.InvariantCultureIgnoreCase))
                return IntTemplate;
            return new DataTemplate();
        }
    }
}
