using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields.Controls
{
    /// <summary>
    /// Interaction logic for ErrorPanel.xaml
    /// </summary>
    public partial class ErrorPanel : UserControl
    {
        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.Register(
            "Errors", typeof(IEnumerable<string>), typeof(ErrorPanel), new PropertyMetadata(default(IEnumerable<string>), ErrorsChanged));

        private static void ErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ErrorPanel panel)) return;
            if (!(e.NewValue is IEnumerable<string> errorList)) return;

            panel.ErrorList.Content = string.Join("\r\n", errorList);
        }

        public IEnumerable<string> Errors
        {
            get => (IEnumerable<string>) GetValue(ErrorsProperty);
            set => SetValue(ErrorsProperty, value);
        }

        public ErrorPanel()
        {
            InitializeComponent();
        }
    }
}
