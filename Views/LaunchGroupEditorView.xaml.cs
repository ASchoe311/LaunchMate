using LaunchMate.Enums;
using LaunchMate.Models;
using Playnite.SDK;
using Playnite;
using Playnite.SDK.Models;
using Playnite.SDK.Controls;
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
using System.Windows.Threading;
using LaunchMate.ViewModels;

namespace LaunchMate.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LaunchGroupEditorView : UserControl
    {
        private DataGridCell _previousLastCell = null;
        private int _previousItemCount;
        private DispatcherTimer _debounceTimer;
        private LaunchGroup _group;
        private ActionType _lastActionType;


        public LaunchGroupEditorView()
        {
            InitializeComponent();
            DataContextChanged += LaunchGroupEditorView_DataContextChanged;
        }

        public LaunchGroupEditorView(LaunchGroup group) : this()
        {
            DataContext = new LaunchGroupEditorViewModel(group);
        }

        private void LaunchGroupEditorView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is LaunchGroupEditorViewModel viewModel)
            {
                _group = viewModel.Group;
                _lastActionType = _group.ActionType;
                _group.Conditions.CollectionChanged += Items_CollectionChanged;
                _previousItemCount = _group.Conditions.Count;

                _debounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                _debounceTimer.Tick += DebounceTimer_Tick;
            }
        }

            void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HandleVisibility();
        }

        /// <summary>
        /// <see cref="System.Collections.Specialized.NotifyCollectionChangedEventHandler"/>
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e"><see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/></param>
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        /// <summary>
        /// On tick, check if number of ConditionGroups in the LaunchGroup has changed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e"><see cref="EventArgs"/></param>
        public void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();

            if (_previousItemCount != _group.Conditions.Count)
            {
                UpdateLastRowCellVisibility();
                _previousItemCount = _group.Conditions.Count;
            }
        }

        /// <summary>
        /// When a row is added or removed from the DataGrid, ensure that the last column of the last row is made invisible
        /// This provides visual clarity to the purpose of the "Next Logical Operator" column
        /// </summary>
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

        /// <summary>
        /// When the DataGrid is first loaded, ensure that the last column of the last row is made invisible
        /// This provides visual clarity to the purpose of the "Next Logical Operator" column
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e"><see cref="DataGridRowEventArgs"/></param>
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

        private void ChangeActionType()
        {
            switch (ActionTypeSelect.SelectedValue)
            {
                case ActionType.App:
                    _group.Action = new AppAction
                    {
                        Target = _group.Action.Target ?? string.Empty
                    };
                    break;
                case ActionType.Web:
                    _group.Action = new WebAction
                    {
                        Target = _group.Action.Target ?? string.Empty
                    };
                    break;
                case ActionType.Script:
                    _group.Action = new ScriptAction
                    {
                        Target = _group.Action.Target ?? string.Empty
                    };
                    _group.AutoClose = false;
                    break;
                case ActionType.Close:
                    _group.Action = new CloseAction
                    {
                        Target = _group.Action.Target ?? string.Empty
                    };
                    _group.AutoClose = false;
                    break;
            }
        }

        private void HandleVisibility()
        {
            switch (ActionTypeSelect.SelectedValue)
            {
                case ActionType.App:
                    AppSelectBtn.Visibility = Visibility.Visible;
                    ScriptSelectBtn.Visibility = Visibility.Hidden;
                    TargetText.Text = "Executable Path: ";
                    ArgsText.Text = "Executable Args: ";
                    ArgsGrid.Visibility = Visibility.Visible;
                    AutoCloseGrid.Visibility = Visibility.Visible;
                    break;
                case ActionType.Web:
                    AppSelectBtn.Visibility = Visibility.Hidden;
                    ScriptSelectBtn.Visibility = Visibility.Hidden;
                    TargetText.Text = "Web URL:  https://";
                    ArgsGrid.Visibility = Visibility.Hidden;
                    AutoCloseGrid.Visibility = Visibility.Visible;
                    break;
                case ActionType.Script:
                    AppSelectBtn.Visibility = Visibility.Hidden;
                    ScriptSelectBtn.Visibility = Visibility.Visible;
                    TargetText.Text = "Script Path: ";
                    ArgsGrid.Visibility = Visibility.Visible;
                    ArgsText.Text = "Script Args: ";
                    AutoCloseGrid.Visibility = Visibility.Hidden;
                    break;
                case ActionType.Close:
                    AppSelectBtn.Visibility = Visibility.Hidden;
                    ScriptSelectBtn.Visibility = Visibility.Hidden;
                    TargetText.Text = "Program Name:";
                    ArgsGrid.Visibility = Visibility.Hidden;
                    AutoCloseGrid.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void ActionTypeSelect_DropDownClosed(object sender, EventArgs e)
        {
            if ((ActionType)ActionTypeSelect.SelectedValue != _lastActionType)
            {
                _lastActionType = (ActionType)ActionTypeSelect.SelectedValue;
                ChangeActionType();
                HandleVisibility();
            }

        }
    }
}
