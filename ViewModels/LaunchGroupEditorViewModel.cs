using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaunchMate.Enums;
using Playnite.SDK;
using LaunchMate.Models;
using LaunchMate.Views;
using LaunchMate.Utilities;
using System.Security.Principal;
using System.Windows;
using Playnite.SDK.Data;

namespace LaunchMate.ViewModels
{
    public class LaunchGroupEditorViewModel : ObservableObject
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private LaunchGroup _group;

        public LaunchGroupEditorViewModel(LaunchGroup group)
        {
            _group = group;
        }

        public LaunchGroup Group { get => _group; set => SetValue(ref _group, value); }

        /// <summary>
        /// Dictionary to convert frontend logic types to <see cref="JoinType"/> enums
        /// </summary>
        public Dictionary<string, JoinType> JoinMethodsDict { get; } = new Dictionary<string, JoinType>()
        {
            { ResourceProvider.GetString("LOCLaunchMateAnd"), JoinType.And },
            { ResourceProvider.GetString("LOCLaunchMateOr"), JoinType.Or },
            { ResourceProvider.GetString("LOCLaunchMateXor"), JoinType.Xor }
        };

        /// <summary>
        /// Command to select an executable for the <see cref="LaunchGroup"/> using a file picker dialog
        /// </summary>
        public RelayCommand SelectAppCmd
        {
            get => new RelayCommand(() =>
            {
                Tuple<string, string, string> app = AppSelector.SelectApp();
                if (app == null)
                {
                    return;
                }
                Group.AppExePath = app.Item1;
                Group.AppExeArgs = app.Item2;
                Group.LnkName = app.Item3;
            });
        }

        /// <summary>
        /// Command to create a new <see cref="ConditionGroup"/> within the current <see cref="LaunchGroup"/>
        /// and open a window to edit it
        /// </summary>
        public RelayCommand AddConditionGroupCmd
        {
            get => new RelayCommand(() =>
            {
                var conditionGroup = new ConditionGroup();

                var window = ConditionGroupEditorViewModel.GetWindow(conditionGroup);

                if (window == null)
                {
                    return;
                }

                if (!(window.ShowDialog() ?? false))
                {
                    return;
                }

                Group.ConditionGroups.Add(conditionGroup);
            });
        }

        /// <summary>
        /// Command to open the editing window for the selected <see cref="ConditionGroup"/>
        /// </summary>
        public RelayCommand<object> EditConditionGroupCmd
        {
            get => new RelayCommand<object>((grp) =>
            {
                if (grp == null)
                {
                    return;
                }
                var grpOriginal = (ConditionGroup)grp;
                var toEdit = Serialization.GetClone(grpOriginal);

                var window = ConditionGroupEditorViewModel.GetWindow(toEdit);

                if (window == null)
                {
                    return;
                }

                if (!(window.ShowDialog() ?? false))
                {
                    return;
                }

                Group.ConditionGroups.Remove(grpOriginal);
                Group.ConditionGroups.Add(toEdit);

            });
        }

        /// <summary>
        /// Command to remove the selected <see cref="ConditionGroup"/> from the current <see cref="LaunchGroup"/>
        /// </summary>
        public RelayCommand<ConditionGroup> RemoveConditionGroupCmd
        {
            get => new RelayCommand<ConditionGroup>((a) =>
            {
                if (a == null) { return; }
                Group.ConditionGroups.Remove(a);
            });
        }

        /// <summary>
        /// Command to launch a window to show the list of games matched by the current <see cref="LaunchGroup"/>
        /// </summary>
        public RelayCommand<LaunchGroup> ShowMatchesCmd
        {
            get => new RelayCommand<LaunchGroup>((grp) =>
            {

                if (grp == null)
                {
                    return;
                }
                var window = MatchedGamesViewModel.GetWindow(grp);
                if (window == null)
                {
                    return;
                }
                if (!(window.ShowDialog() ?? false))
                {
                    return;
                }

            });
        }

        /// <summary>
        /// Creates the window for displaying the launch group editor
        /// </summary>
        /// <param name="launchGroup"><see cref="LaunchGroup"/> to edit</param>
        /// <returns>A <see cref="Window"/> with Content=<see cref="LaunchGroupEditorView"/>(<paramref name="launchGroup"/>) 
        /// and DataContext=<see cref="LaunchGroupEditorViewModel"/>(<paramref name="launchGroup"/>)</returns>
        public static Window GetWindow(LaunchGroup launchGroup)
        {
            try
            {
                var viewModel = new LaunchGroupEditorViewModel(launchGroup);
                var launchGroupEditorView = new LaunchGroupEditorView(launchGroup.ConditionGroups);
                var window = WindowHelper.CreateSizedWindow
                (
                    ResourceProvider.GetString("LOCLaunchMateLaunchGroupEditorTitle"), 800, 650
                );
                window.Content = launchGroupEditorView;
                window.DataContext = viewModel;
                return window;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception occurred during initialization of edit window");
                return null;
            }
        }

        /// <summary>
        /// Saves the edited <see cref="LaunchGroup"/> and closes the window
        /// </summary>
        public RelayCommand<Window> SaveCommand
        {
            get => new RelayCommand<Window>((w) =>
            {
                if (Group.AppExePath == string.Empty)
                {
                    API.Instance.Dialogs.ShowMessage(
                        ResourceProvider.GetString("LOCLaunchMateNoExe"), string.Empty,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (Group.Enabled && Group.ConditionGroups.Count == 0)
                {
                    if (API.Instance.Dialogs.ShowMessage(
                        ResourceProvider.GetString("LOCLaunchMateNoConditions"),
                        string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                w.DialogResult = true;
                w.Close();
            });
        }

    }
}
