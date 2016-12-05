using System.Windows;
using System.Windows.Controls;
using FluentAssertions;

namespace NZazu.Fields.Controls
{
    /// <summary>
    /// Interaction logic for DynamicDataTable.xaml
    /// </summary>
    public partial class DynamicDataTable
    {
        // Expose the grid & panel
        public Grid Grid => LayoutGrid;
        public StackPanel Panel => ButtonPanel;

        public DynamicDataTable()
        {
            InitializeComponent();
        }
    }
}