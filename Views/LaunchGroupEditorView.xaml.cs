using LaunchMate.Models;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaunchMate.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LaunchGroupEditorView : UserControl
    {
        private DataGridCell _previousLastCell = null;
        private ObservableCollection<ConditionGroup> _conditionGroups;
        public LaunchGroupEditorView(ObservableCollection<ConditionGroup> conditionGroups)
        {
            InitializeComponent();
            _conditionGroups = conditionGroups;
            _conditionGroups.CollectionChanged += Items_CollectionChanged;

        }
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateLastRowCellVisibility();
        }

        private void UpdateLastRowCellVisibility()
        {
            var lastRow = GridConditionGroups.Items.Count - 1;
            var lastColumnIndex = GridConditionGroups.Columns.Count - 1;

            // Make the old last cell visible again if it exists
            if (_previousLastCell != null)
            {
                _previousLastCell.Visibility = Visibility.Visible;
            }

            if (lastRow >= 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var lastRowItem = GridConditionGroups.Items[lastRow];
                    var lastRowElement = GridConditionGroups.ItemContainerGenerator.ContainerFromItem(lastRowItem) as DataGridRow;
                    if (lastRowElement != null)
                    {
                        var cell = GridConditionGroups.Columns[lastColumnIndex].GetCellContent(lastRowElement)?.Parent as System.Windows.Controls.DataGridCell;
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
