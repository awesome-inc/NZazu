using System.Windows;

namespace NZazuFiddle
{
    public partial class PreviewView
    {
        public PreviewView()
        {
            InitializeComponent();
        }

        private void ApplyChanges(object sender, RoutedEventArgs e)
        {
            View.ApplyChanges();
        }
    }
}
