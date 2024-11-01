using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using LaunchMate.Models;
using LaunchMate.Views;
using LaunchMate.ViewModels;
using System.Linq;
using System.Text;
using System.Management;
using LaunchMate.Utilities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Playnite.SDK.Models;
using System.Diagnostics;

namespace LaunchMate
{
    public class LaunchMate : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private SettingsViewModel settings { get; set; }

        internal Settings Settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("61d7fcec-322d-4eb6-b981-1c8f8122ddc8");

        private readonly int vNum = 2;
        private readonly SidebarItem launchGroupsSidebarItem;

        public static Cache Cache;

        public LaunchMate(IPlayniteAPI api) : base(api)
        {
            settings = new SettingsViewModel(this);
            Settings = settings.Settings;
            settings.SettingsUpdated += OnSettingsUpdated;
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
            Cache = new Cache();

            launchGroupsSidebarItem = new SidebarItem
            {
                Title = "LaunchMate",
                Icon = Path.Combine(Path.GetDirectoryName(typeof(LaunchMate).Assembly.Location), "icon-gray.png"),
                Type = SiderbarItemType.Button,
                Activated = () => this.OpenSettingsView(),
                ProgressValue = 0,
                ProgressMaximum = 100
            };
        }

        private void OnSettingsUpdated(object sender, EventArgs e)
        {
            Settings = settings.Settings;
        }

        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            yield return launchGroupsSidebarItem;
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            //MigrateSettings();
            if (settings.Settings.PluginVersion == 1)
            {
                PlayniteApi.Dialogs.ShowMessage(
                    "Thank you for installing LaunchMate 2.0!\nThere have been many changes, and unfortunately the new system is not 100% compatible with the old settings. The plugin will do its best to migrate your old settings, but some issues may occur.\n\nYour old plugin settings have been saved in the plugin data directory.\n\nI'm very sorry for the inconvenience.",
                    "LaunchMate 2.0", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation
                    );
                ConvertOldSettings();
            }
            else if (settings.Settings.ShowFirstRunMessage)
            {
                List<MessageBoxOption> options = new List<MessageBoxOption>()
                {
                    new MessageBoxOption("Ok", isDefault: true, isCancel:true),
                    new MessageBoxOption("Show Guide")
                };
                MessageBoxOption chosen = PlayniteApi.Dialogs.ShowMessage("Thank you for installing LaunchMate! To control the plugin, just click the rocket icon in the left sidebar. For help, click \"Show Guide\".\n\nThis message will not be shown again.", "LaunchMate First Run", System.Windows.MessageBoxImage.Information, options);
                if (!chosen.IsDefault && !chosen.IsCancel)
                {
                    Process.Start("https://github.com/ASchoe311/LaunchMate?tab=readme-ov-file#usage");
                }
                settings.Settings.ShowFirstRunMessage = false;
                SavePluginSettings(settings.Settings);
            }
        }

        public void ConvertOldSettings()
        {
            var settingsFile = Path.Combine(GetPluginUserDataPath(), "config.json");
            OldSettings oldSettings;
            var tempFile = Path.Combine(GetPluginUserDataPath(), "config.old.json");
            string jsonStr = File.ReadAllText(settingsFile);
            using (FileStream fs = File.Create(tempFile))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(jsonStr);
                }
            }
            oldSettings = JsonConvert.DeserializeObject<OldSettings>(jsonStr);

            Settings newSettings = new Settings();

            newSettings.PluginVersion = 2;
            int i = 0;
            foreach (var group in oldSettings.Groups)
            {
                string groupName = $"Unnamed Group ({i})";

                AppAction act = new AppAction
                {
                    Target = group.LaunchTargetUri,
                    TargetArgs = group.AppExeArgs,
                };
                
                ObservableCollection<LaunchCondition> conditions = new ObservableCollection<LaunchCondition>();

                foreach (var condGroup in group.ConditionGroups)
                {
                    bool thisNot = condGroup.Not;

                    for (int j = 0; j < condGroup.Conditions.Count; j++)
                    {
                        var cond = condGroup.Conditions[j];
                        bool not = cond.Not ^ condGroup.Not;
                        var joiner = j < condGroup.Conditions.Count - 1 ? cond.Joiner : condGroup.Joiner;


                        conditions.Add(new LaunchCondition
                        {
                            Not = not,
                            Filter = cond.Filter,
                            FilterId = null,
                            FilterType = cond.FilterType,
                            FuzzyMatch = cond.FuzzyMatch,
                            Joiner = joiner,
                        });

                    }

                }

                newSettings.Groups.Add(new LaunchGroup
                {
                    Action = act,
                    Enabled = group.Enabled.HasValue ? group.Enabled.Value : true,
                    ActionType = Enums.ActionType.App,
                    AutoClose = group.AutoClose.HasValue ? group.AutoClose.Value : true,
                    LaunchDelay = group.LaunchDelay.HasValue ? group.LaunchDelay.Value : 0,
                    Name = groupName,
                    Conditions = conditions,
                    
                });


                i += 1;
            }

            settings.Settings = newSettings;
            SavePluginSettings(settings.Settings);
        }

        private Stack<LaunchGroup> toClose = new Stack<LaunchGroup>();

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            base.OnGameStarting(args);

            // Dispatch group handling to async function
            foreach (var group in Settings.Groups)
            {
                if (!group.Enabled)
                {
                    continue;
                }
                LaunchGroupDispatcher(args, group);
            }
        }

        /// <summary>
        /// Asynchronously dispatches a launch group to prevent sleep timers from blocking each other
        /// </summary>
        /// <param name="args"><see cref="OnGameStartingEventArgs"/></param>
        /// <param name="group">The <see cref="LaunchGroup"/> to dispatch</param>
        private async void LaunchGroupDispatcher(OnGameStartingEventArgs args, LaunchGroup group)
        {
            await Task.Run(() => HandleLaunchGroup(args, group));
        }

        /// <summary>
        /// Handles execution of a launch group
        /// </summary>
        /// <param name="args"><see cref="OnGameStartingEventArgs"/></param>
        /// <param name="group">The launchgroup to execute</param>
        private void HandleLaunchGroup(OnGameStartingEventArgs args, LaunchGroup group)
        {

            logger.Debug($"Checking launch group conditions for group named {group.Name}");
            
            if (group.MeetsConditions(args.Game))
            {
                logger.Debug($"Conditions passed, waiting {group.LaunchDelay} ms to execute action for group \"{group.Name}\"");
                System.Threading.Thread.Sleep(group.LaunchDelay);
                
                if (group.Action.Execute(group.Name) && group.AutoClose){
                    toClose.Push(group);
                }
            }

        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            base.OnGameStopped(args);
            while (toClose.Count > 0)
            {
                LaunchGroup group = toClose.Pop();

                //logger.Debug($"{group.Name} - Trying to close {group.Action.Target}");

                group.Action.AutoClose(group.Name);

            }
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new SettingsView();
        }
    }
}