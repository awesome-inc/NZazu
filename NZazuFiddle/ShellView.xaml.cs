using System.Windows;

namespace NZazuFiddle
{
    // ReSharper disable once UnusedMember.Global
    public partial class ShellView
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private void ApplyChanges(object sender, RoutedEventArgs e)
        {
            View.ApplyChanges();
        }
    }
}
