using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SideLauncher.Enums;
using Playnite.SDK;
using SideLauncher.Models;
using SideLauncher.Views;
using SideLauncher.Utilities;
using System.Security.Principal;
using System.Windows;

namespace SideLauncher.ViewModels
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

        public RelayCommand AddConditionCmd
        {
            get => new RelayCommand(() =>
            {
                Group.Conditions.Add(new LaunchCondition());
            });
        }

        public RelayCommand<LaunchCondition> RemoveConditionCmd
        {
            get => new RelayCommand<LaunchCondition>((c) =>
            {
                if (c != null)
                {
                    Group.Conditions.Remove(c);
                }
            });
        }

        public Dictionary<string, JoinType> JoinMethodsDict { get; } = new Dictionary<string, JoinType>()
        {
            { "Execute if all conditions are met (AND)", JoinType.And },
            { "Execute if any condition is met (OR)", JoinType.Or },
            { "Execute if only one condition is met (XOR)", JoinType.Xor }
        };

        public static Dictionary<string, FilterTypes> FilterTypesDict { get; } = new Dictionary<string, FilterTypes>
        {
            { "All Games", FilterTypes.All },
            { "Name", FilterTypes.Name },
            { "Source", FilterTypes.Source },
            { "Developer", FilterTypes.Developers },
            { "Publisher", FilterTypes.Publishers },
            { "Category", FilterTypes.Categories },
            { "Genre", FilterTypes.Genres },
            { "Game ID", FilterTypes.Gameid },
            { "Feature", FilterTypes.Features },
            { "Tag", FilterTypes.Tags },
            { "Platform", FilterTypes.Platforms },
            { "Series", FilterTypes.Series }
        };

        public static Window GetWindow(Settings settings, LaunchGroup launchGroup)
        {
            try
            {
                var viewModel = new LaunchGroupEditorViewModel(settings, launchGroup);
                var launchGroupEditorView = new LaunchGroupEditorView();
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
                if (Group.Enabled && Group.Conditions.Count == 0)
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
