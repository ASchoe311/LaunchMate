using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LaunchMate.Enums;
using System.Windows.Threading;
using LaunchMate.Models;
using LaunchMate.ViewModels;
using LaunchMate.Utilities;
using Playnite.SDK;
using System.Windows.Navigation;

namespace LaunchMate.Views
{
    public partial class SettingsView : UserControl
    {
        private DataGridCell _previousLastCell = null;
        private int _previousItemCount;
        private DispatcherTimer _debounceTimer;
        private LaunchGroup _group;
        private ActionType _lastActionType;

        public SettingsView()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Select first launch group when window is loaded
        /// </summary>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            nameList.SelectedIndex = 0;
            if (nameList.SelectedItem is LaunchGroup g)
            {
                var set = DataContext as SettingsViewModel;
                var groups = set.Settings.Groups;
                var selectedGroup = groups.Where((x) => x.Name == g.Name).First();
                groupView.DataContext = selectedGroup;
                _group = selectedGroup;
                GroupChanged();
                HandleVisibility();
            }
            if (nameList.Items.Count == 0)
            {
                groupView.Visibility = Visibility.Collapsed;
                noGroupsView.Visibility = Visibility.Visible;
            }
        }
        
        /// <summary>
        /// Ensure event handlers are set for the currently selected LaunchGroup
        /// </summary>
        private void GroupChanged()
        {
            _lastActionType = _group.ActionType;
            _group.Conditions.CollectionChanged += Items_CollectionChanged;
            _previousItemCount = _group.Conditions.Count;

            // Use debounce timer to check if number of items in collection has changed after 100 ms
            // Do this because otherwise and edit event would trigger UpdateLastRowCellVisibility()
            _debounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _debounceTimer.Tick += DebounceTimer_Tick;
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

        /// <summary>
        /// Change visibility of certain elements based on action type
        /// </summary>
        private void HandleVisibility()
        {
            switch (ActionTypeSelect.SelectedValue)
            {
                case ActionType.App:
                    AppSelectBtn.Visibility = Visibility.Visible;
                    ScriptSelectBtn.Visibility = Visibility.Hidden;
                    TargetText.Text = "App Path: ";
                    ArgsText.Text = "App Parameters: ";
                    r0c1Args.Visibility = Visibility.Visible;
                    r0c1Web.Visibility = Visibility.Hidden;
                    AutoCloseGrid.Visibility = Visibility.Visible;
                    break;
                case ActionType.Web:
                    AppSelectBtn.Visibility = Visibility.Hidden;
                    ScriptSelectBtn.Visibility = Visibility.Hidden;
                    TargetText.Text = "Web URL: ";
                    r0c1Args.Visibility = Visibility.Hidden;
                    r0c1Web.Visibility = Visibility.Visible;
                    AutoCloseGrid.Visibility = Visibility.Visible;
                    break;
                case ActionType.Script:
                    AppSelectBtn.Visibility = Visibility.Hidden;
                    ScriptSelectBtn.Visibility = Visibility.Visible;
                    TargetText.Text = "Script: ";
                    r0c1Args.Visibility = Visibility.Visible;
                    ArgsText.Text = "Script Arguments: ";
                    r0c1Web.Visibility = Visibility.Hidden;
                    AutoCloseGrid.Visibility = Visibility.Hidden;
                    break;
                case ActionType.Close:
                    AppSelectBtn.Visibility = Visibility.Hidden;
                    ScriptSelectBtn.Visibility = Visibility.Hidden;
                    TargetText.Text = "Program Name: ";
                    r0c1Args.Visibility = Visibility.Hidden;
                    r0c1Web.Visibility = Visibility.Hidden;
                    AutoCloseGrid.Visibility = Visibility.Hidden;
                    break;
            }
        }

        /// <summary>
        /// Cast the action of the selected group to the correct type based on ActionType
        /// </summary>
        private void ChangeActionType()
        {
            switch (ActionTypeSelect.SelectedValue)
            {
                case ActionType.App:
                    _group.Action = new AppAction
                    {
                        Target = _group.Action.Target ?? string.Empty,
                        TargetArgs = _group.Action.TargetArgs ?? string.Empty,
                    };
                    break;
                case ActionType.Web:
                    _group.Action = new WebAction
                    {
                        Target = _group.Action.Target ?? string.Empty,
                        TargetArgs = _group.Action.TargetArgs ?? string.Empty,
                    };
                    break;
                case ActionType.Script:
                    _group.Action = new ScriptAction
                    {
                        Target = _group.Action.Target ?? string.Empty,
                        TargetArgs = _group.Action.TargetArgs ?? string.Empty,
                    };
                    break;
                case ActionType.Close:
                    _group.Action = new CloseAction
                    {
                        Target = _group.Action.Target ?? string.Empty,
                        TargetArgs = _group.Action.TargetArgs ?? string.Empty,
                    };
                    break;
            }
        }


        /// <summary>
        /// Set data context for the group editor view to the selected LaunchGroup
        /// </summary>
        private void NameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (nameList.SelectedItem is LaunchGroup g)
            {
                var set = DataContext as SettingsViewModel;
                var groups = set.Settings.Groups;
                var selectedGroup = groups.Where((x) => x.Name == g.Name).First();
                groupView.DataContext = selectedGroup;
                _group = selectedGroup;
                HandleVisibility();
                GroupChanged();
            }
        }

        /// <summary>
        /// Handle changes based on action type selection
        /// </summary>
        private void ActionTypeSelect_DropDownClosed(object sender, EventArgs e)
        {
            if ((ActionType)ActionTypeSelect.SelectedValue != _lastActionType)
            {
                _lastActionType = (ActionType)ActionTypeSelect.SelectedValue;
                ChangeActionType();
                HandleVisibility();
            }

        }

        /// <summary>
        /// Event handler for clicking the "View Matched Games" button
        /// </summary>
        private void SeeMatches_Click(object sender, RoutedEventArgs e)
        {
            var window = MatchedGamesViewModel.GetWindow(_group);
            if (window == null)
            {
                return;
            }
            if (!(window.ShowDialog() ?? false))
            {
                return;
            }
        }
        /// <summary>
        /// Event handler for adding a new condition to a group
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _group.Conditions.Add(new LaunchCondition());
        }

        /// <summary>
        /// Event handler for removing a condition from a group
        /// </summary>
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (GridConditions.SelectedItem != null)
            {
                if (GridConditions.SelectedItem is LaunchCondition g)
                {
                    _group.Conditions.Remove(g);
                }
            }
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var set = DataContext as SettingsViewModel;
            var groups = set.Settings.Groups;
            groups.Add(new LaunchGroup()
            {
                Name = $"Unnamed Group ({groups.Count})"
            });
            if (nameList.Items.Count != 0)
            {
                groupView.Visibility = Visibility.Visible;
                noGroupsView.Visibility = Visibility.Collapsed;
            }
            nameList.SelectedIndex = nameList.Items.Count - 1;
            if (nameList.SelectedItem is LaunchGroup g)
            {
                var selectedGroup = groups.Where((x) => x.Name == g.Name).First();
                groupView.DataContext = selectedGroup;
                _group = selectedGroup;
                GroupChanged();
            }
        }
        private void RemoveGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameList.SelectedItem == null)
            {
                return;
            }
            int index = nameList.SelectedIndex;
            if (nameList.SelectedItem is LaunchGroup a)
            {
                var set = DataContext as SettingsViewModel;
                var groups = set.Settings.Groups;
                //var selectedGroup = groups.Where((x) => x.Name == g.Name).First();
                groups.Remove(a);
            }
            if (nameList.Items.Count == 0)
            {
                groupView.Visibility = Visibility.Collapsed;
                noGroupsView.Visibility = Visibility.Visible;
                return;
            }
            nameList.SelectedIndex = ((index - 1) >= 0) ? index - 1 : 0;
            if (nameList.SelectedItem is LaunchGroup g)
            {
                var set = DataContext as SettingsViewModel;
                var groups = set.Settings.Groups;
                var selectedGroup = groups.Where((x) => x.Name == g.Name).First();
                groupView.DataContext = selectedGroup;
                _group = selectedGroup;
                GroupChanged();
            }
            HandleVisibility();
        }

        private void AppSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            Tuple<string, string, string> app = AppSelector.SelectApp();
            if (app == null)
            {
                return;
            }
            _group.Action.Target = app.Item1;
            _group.Action.TargetArgs = app.Item2;
        }

        private void ScriptSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            string file = API.Instance.Dialogs.SelectFile("Script File|*.bat");
            if (file != null)
            {
                _group.Action.Target = file;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}