using System.Windows;
using System.Windows.Controls;

namespace NZazu.Fields.Controls
{
    /// <summary>
    /// Interaction logic for DynamicDataTable.xaml
    /// </summary>
    public partial class DynamicDataTable
    {
        public static readonly DependencyProperty ValuesAsJsonProperty = DependencyProperty.Register(
            "ValuesAsJson", typeof(string), typeof(DynamicDataTable), new PropertyMetadata(default(string)));

        public string ValuesAsJson
        {
            get { return (string)GetValue(ValuesAsJsonProperty); }
            set { SetValue(ValuesAsJsonProperty, value); }
        }

        public DynamicDataTable()
        {
            InitializeComponent();
        }
    }
}
