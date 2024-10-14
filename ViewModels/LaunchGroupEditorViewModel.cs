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
        private readonly Settings _settings;
        private LaunchGroup _group;

        public LaunchGroupEditorViewModel(Settings settings, LaunchGroup group)
        {
            _settings = settings;
            _group = group;
        }

        public LaunchGroup Group { get => _group; set => SetValue(ref _group, value); }

        public Dictionary<string, JoinType> JoinMethodsDict { get; } = new Dictionary<string, JoinType>()
        {
            { "AND", JoinType.And },
            { "OR", JoinType.Or },
            { "XOR", JoinType.Xor }
        };

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

        public RelayCommand AddConditionGroupCmd
        {
            get => new RelayCommand(() =>
            {
                var conditionGroup = new ConditionGroup();

                var window = ConditionGroupEditorViewModel.GetWindow(_settings, conditionGroup);

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

                var window = ConditionGroupEditorViewModel.GetWindow(_settings, toEdit);

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

        public RelayCommand<ConditionGroup> RemoveConditionGroupCmd
        {
            get => new RelayCommand<ConditionGroup>((a) =>
            {
                if (a == null) { return; }
                Group.ConditionGroups.Remove(a);
            });
        }

        public RelayCommand<LaunchGroup> ShowMatchesCmd
        {
            get => new RelayCommand<LaunchGroup>((grp) =>
            {

                if (grp == null)
                {
                    return;
                }
                var window = MatchedGamesViewModel.GetWindow(_settings, grp);
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

        public static Window GetWindow(Settings settings, LaunchGroup launchGroup)
        {
            try
            {
                var viewModel = new LaunchGroupEditorViewModel(settings, launchGroup);
                var launchGroupEditorView = new LaunchGroupEditorView(launchGroup.ConditionGroups);
                var window = WindowHelper.CreateSizedWindow
                (
                    "Launch Group Editor", 800, 600
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

        public RelayCommand<Window> SaveCommand
        {
            get => new RelayCommand<Window>((w) =>
            {
                if (Group.AppExePath == string.Empty)
                {
                    API.Instance.Dialogs.ShowMessage(
                        "Application executable path cannot be empty", string.Empty,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (Group.Enabled && Group.ConditionGroups.Count == 0)
                {
                    if (API.Instance.Dialogs.ShowMessage(
                        "No conditions were set. This will result in the application launching alongside all games. Continue?",
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
