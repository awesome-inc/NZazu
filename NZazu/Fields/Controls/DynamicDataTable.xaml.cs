using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            get
            {
                var data = new Dictionary<string, string>();
                foreach (Control child in LayoutGrid.Children)
                {
                    data.Add(child.Name, string.Empty);
                }
                return data.ToString();
                return (string)GetValue(ValuesAsJsonProperty);
            }
            set { SetValue(ValuesAsJsonProperty, value); }
        }

        // Expose the layout grid
        public Grid Grid => this.LayoutGrid;

        public DynamicDataTable()
        {
            InitializeComponent();

            ValuesAsJson = "{ 'name':'data table'}";
        }

        // http://stackoverflow.com/questions/15686381/wpf-iterate-through-datagrid
        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (itemsSource == null) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }
    }
}