using Playnite.SDK;
using Playnite.SDK.Data;
using System.Collections.Generic;
using LaunchMate.Models;
using LaunchMate.Enums;
using System;

namespace LaunchMate.ViewModels
{

    public class SettingsViewModel : ObservableObject, ISettings
    {
        private readonly LaunchMate plugin;
        private Settings EditingClone { get; set; }

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

        public event EventHandler SettingsUpdated;

        public SettingsViewModel(LaunchMate plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<Settings>();

            // LoadPluginSettings returns null if no saved data is available.
            Settings = savedSettings ?? new Settings();
            FixActionTypes();
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
            { ResourceProvider.GetString("LOCSeriesLabel"), FilterTypes.Series },
            { "Executable Running", FilterTypes.ExeName },
            { "Process Running", FilterTypes.Process },
            { "Service Running", FilterTypes.Service },
        };

        public Dictionary<string, ActionType> ActionTypesDict { get; } = new Dictionary<string, ActionType>()
        {
            { "Launch an App", ActionType.App },
            { "Open a Webpage", ActionType.Web },
            { "Run a Script", ActionType.Script },
            { "Start a Service", ActionType.StartService },
            { "Stop a Process", ActionType.Close },
            { "Stop a Service", ActionType.Stop },

        };

        private void FixActionTypes()
        {
            foreach (var group in Settings.Groups)
            {
                switch (group.ActionType)
                {
                    case ActionType.App:
                        group.Action = new AppAction
                        {
                            Target = group.Action.Target,
                            TargetArgs = group.Action.TargetArgs
                        };
                        break;
                    case ActionType.Web:
                        group.Action = new WebAction
                        {
                            Target = group.Action.Target,
                            UseWebView = group.Action.UseWebView,
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
                    case ActionType.StartService:
                        group.Action = new StartServiceAction
                        {
                            Target = group.Action.Target
                        };
                        break;
                    case ActionType.Stop:
                        group.Action = new StopServiceAction
                        {
                            Target = group.Action.Target
                        };
                        break;
                    default: break;
                }
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            EditingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            Settings = EditingClone;
            FixActionTypes();
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            plugin.SavePluginSettings(Settings);
            FixActionTypes();
            SettingsUpdated?.Invoke(this, EventArgs.Empty); // Notify that settings are updated
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            foreach (var group in Settings.Groups)
            {
                if (group.Action.Target == null ||  group.Action.Target == string.Empty)
                {
                    errors.Add($"Error: Launch Group {group.Name} does not have an action target\n");
                }
            }
            return errors.Count == 0;
        }
    }
}