using LaunchMate.Enums;
using LaunchMate.Models;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchMate.ViewModels
{
    public class LaunchMatePanelViewModel : ObservableObject
    {
        private readonly LaunchMate plugin;

        private Settings _settings;
        public Settings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        public LaunchMatePanelViewModel(LaunchMate plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<Settings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
                FixActionTypes();
            }
            else
            {
                Settings = new Settings();
            }
        }

        public Dictionary<string, JoinType> JoinMethodsDict { get; } = new Dictionary<string, JoinType>()
        {
            { ResourceProvider.GetString("LOCLaunchMateAnd"), JoinType.And },
            { ResourceProvider.GetString("LOCLaunchMateOr"), JoinType.Or },
            //{ ResourceProvider.GetString("LOCLaunchMateXor"), JoinType.Xor }
        };

        /// <summary>
        /// Dictionary to convert frontend filter types string representation to <see cref="FilterTypes"/> enums
        /// </summary>
        public static Dictionary<string, FilterTypes> FilterTypesDict { get; } = new Dictionary<string, FilterTypes>
        {
            { ResourceProvider.GetString("LOCAllGames"), FilterTypes.All },
            { ResourceProvider.GetString("LOCNameLabel"), FilterTypes.Name },
            { ResourceProvider.GetString("LOCSourceLabel"), FilterTypes.Source },
            { ResourceProvider.GetString("LOCDeveloperLabel"), FilterTypes.Developers },
            { ResourceProvider.GetString("LOCPublisherLabel"), FilterTypes.Publishers },
            { ResourceProvider.GetString("LOCCategoryLabel"), FilterTypes.Categories },
            { ResourceProvider.GetString("LOCGenreLabel"), FilterTypes.Genres },
            { ResourceProvider.GetString("LOCGameId"), FilterTypes.GameId },
            { ResourceProvider.GetString("LOCFeatureLabel"), FilterTypes.Features },
            { ResourceProvider.GetString("LOCTagLabel"), FilterTypes.Tags },
            { ResourceProvider.GetString("LOCPlatformTitle"), FilterTypes.Platforms },
            { ResourceProvider.GetString("LOCSeriesLabel"), FilterTypes.Series }
        };

        public Dictionary<string, ActionType> ActionTypesDict { get; } = new Dictionary<string, ActionType>()
        {
            { "Launch an App", ActionType.App },
            { "Open a Webpage", ActionType.Web },
            { "Run a Script", ActionType.Script },
            { "Close program", ActionType.Close },

        };

        /// <summary>
        /// Command to create a new <see cref="LaunchGroup"/> and open a window to edit it
        /// </summary>
        public RelayCommand AddLaunchGroupCmd
        {
            get => new RelayCommand(() =>
            {
                var launchGroup = new LaunchGroup();

                var window = LaunchGroupEditorViewModel.GetWindow(launchGroup);

                if (window == null)
                {
                    return;
                }

                if (!(window.ShowDialog() ?? false))
                {
                    return;
                }

                Settings.Groups.Add(launchGroup);
            });
        }

        /// <summary>
        /// Command to remove the selected <see cref="LaunchGroup"/>
        /// </summary>
        public RelayCommand<LaunchGroup> RemoveLaunchGroupCmd
        {
            get => new RelayCommand<LaunchGroup>((a) =>
            {
                if (a == null) { return; }
                Settings.Groups.Remove(a);
            });
        }

        /// <summary>
        /// Command to launch a window to show the list of games matched by the selected <see cref="LaunchGroup"/>
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

        public RelayCommand SaveCmd
        {
            get => new RelayCommand(() =>
            {
                plugin.Settings.Groups = Settings.Groups;
                plugin.SavePluginSettings(Settings);
            });
        }

        private void FixActionTypes()
        {
            foreach (var group in Settings.Groups)
            {
                switch (group.ActionType)
                {
                    case ActionType.Web:
                        group.Action = new WebAction
                        {
                            Target = group.Action.Target
                        };
                        break;
                    case ActionType.Script:
                        group.Action = new ScriptAction
                        {
                            Target = group.Action.Target,
                            TargetArgs = group.Action.TargetArgs
                        };
                        break;
                    case ActionType.Close:
                        group.Action = new CloseAction
                        {
                            Target = group.Action.Target
                        };
                        break;
                    default: break;
                }
            }
        }

    }
    
}
