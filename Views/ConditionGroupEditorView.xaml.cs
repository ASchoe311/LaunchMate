using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaunchMate.Models;
using DataGridCell = System.Windows.Controls.DataGridCell;
using UserControl = System.Windows.Controls.UserControl;

namespace LaunchMate.Views
{
    /// <summary>
    /// Interaction logic for ConditionGroupEditorView.xaml
    /// </summary>
    public partial class ConditionGroupEditorView : UserControl
    {
        private System.Windows.Controls.DataGridCell _previousLastCell = null;
        private ObservableCollection<LaunchCondition> _conditions;

        public ConditionGroupEditorView(ObservableCollection<LaunchCondition> conditions)
        {
            InitializeComponent();
            _conditions = conditions;
            _conditions.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateLastRowCellVisibility();
        }

        private void UpdateLastRowCellVisibility()
        {
            var lastRow = GridConditions.Items.Count - 1;
            var lastColumnIndex = GridConditions.Columns.Count - 1;

            // Make the old last cell visible again if it exists
            if (_previousLastCell != null)
            {
                _previousLastCell.Visibility = Visibility.Visible;
            }

            if (lastRow >= 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var lastRowItem = GridConditions.Items[lastRow];
                    var lastRowElement = GridConditions.ItemContainerGenerator.ContainerFromItem(lastRowItem) as DataGridRow;
                    if (lastRowElement != null)
                    {
                        var cell = GridConditions.Columns[lastColumnIndex].GetCellContent(lastRowElement)?.Parent as System.Windows.Controls.DataGridCell;
                        if (cell != null)
                        {
                            cell.Visibility = Visibility.Hidden;
                            _previousLastCell = cell; // Store the reference to the current last cell
                        }
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var dataGrid = sender as System.Windows.Controls.DataGrid;
            if (dataGrid != null)
            {
                var lastRow = dataGrid.Items.Count - 1;
                var lastColumnIndex = dataGrid.Columns.Count - 1;

                // Make the old last cell visible again if it exists
                if (_previousLastCell != null)
                {
                    _previousLastCell.Visibility = Visibility.Visible;
                }

                if (e.Row.GetIndex() == lastRow)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var cell = dataGrid.Columns[lastColumnIndex].GetCellContent(e.Row)?.Parent as DataGridCell;
                        if (cell != null)
                        {
                            cell.Visibility = Visibility.Hidden;
                            _previousLastCell = cell; // Store the reference to the current last cell
                        }
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }
        }
    }
}
