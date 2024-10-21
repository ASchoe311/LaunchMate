using LaunchMate.Enums;
using LaunchMate.Models;
using LaunchMate.ViewModels;
using System;
using System.Collections.Generic;
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

namespace LaunchMate.Views
{
    /// <summary>
    /// Interaction logic for LaunchMatePanel.xaml
    /// </summary>
    public partial class LaunchMatePanelView : UserControl
    {
        private DataGridCell _previousLastCell = null;
        private int _previousItemCount;
        private DispatcherTimer _debounceTimer;
        private LaunchGroup _group;
        private ActionType _lastActionType;

        private LaunchMatePanelViewModel _viewModel;

        public LaunchMatePanelView(LaunchMatePanelViewModel viewModel)
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            DataContext = viewModel;
            _viewModel = viewModel;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            nameList.SelectedIndex = 0;
            if (nameList.SelectedItem is LaunchGroup g)
            {
                var selectedGroup = _viewModel.Settings.Groups.Where((x) => x.Name == g.Name).First();
                groupView.DataContext = selectedGroup;
                _group = selectedGroup;
            }
            HandleVisibility();
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
                    TargetText.Text = "Script Path: ";
                    r0c1Args.Visibility = Visibility.Visible;
                    ArgsText.Text = "Script Args: ";
                    r0c1Web.Visibility = Visibility.Hidden;
                    AutoCloseGrid.Visibility = Visibility.Hidden;
                    break;
                case ActionType.Close:
                    AppSelectBtn.Visibility = Visibility.Hidden;
                    ScriptSelectBtn.Visibility = Visibility.Hidden;
                    TargetText.Text = "Program Name:";
                    r0c1Args.Visibility = Visibility.Hidden;
                    r0c1Web.Visibility = Visibility.Hidden;
                    AutoCloseGrid.Visibility = Visibility.Hidden;
                    break;
            }
        }

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

        private void NameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (nameList.SelectedItem is LaunchGroup g)
            {
                var selectedGroup = _viewModel.Settings.Groups.Where((x) => x.Name == g.Name).First();
                groupView.DataContext = selectedGroup;
                _group = selectedGroup;
                HandleVisibility();
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
