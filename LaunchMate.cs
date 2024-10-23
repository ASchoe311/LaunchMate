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

namespace LaunchMate
{
    public class LaunchMate : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private SettingsViewModel settings { get; set; }
        internal LaunchMatePanelViewModel launchMatePanelModel { get; set; }

        internal Settings Settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("61d7fcec-322d-4eb6-b981-1c8f8122ddc8");

        private readonly int vNum = 1;

        public static LaunchMate Instance { get; set; }

        public LaunchMatePanelView LaunchGroupsManager { get; set; }
        private readonly SidebarItem launchGroupsSidebarItem;

        public LaunchMate(IPlayniteAPI api) : base(api)
        {
            settings = new SettingsViewModel(this);
            Settings = settings.Settings;
            settings.SettingsUpdated += OnSettingsUpdated;
            launchMatePanelModel = new LaunchMatePanelViewModel(this);
            launchMatePanelModel.PanelUpdated += OnPanelUpdated;
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
            Instance = this;
            launchGroupsSidebarItem = new SidebarItem
            {
                Title = "LaunchMate1",
                Icon = Path.Combine(Path.GetDirectoryName(typeof(LaunchMate).Assembly.Location), "icon.png"),
                Type = SiderbarItemType.View,
                Opened = () => GetLaunchMateManager(),
                ProgressValue = 0,
                ProgressMaximum = 100
            };
        }

        private void OnSettingsUpdated(object sender, EventArgs e)
        {
            Settings = settings.Settings;
        }

        private void OnPanelUpdated(object sender, EventArgs e)
        {
            Settings = launchMatePanelModel.Settings;
        }

        public static LaunchMatePanelView GetLaunchMateManager()
        {
            if (Instance.LaunchGroupsManager == null)
            {
                Instance.LaunchGroupsManager = new LaunchMatePanelView()
                {
                    DataContext = Instance.launchMatePanelModel
                };
            }
            return Instance.LaunchGroupsManager;
        }

        public override IEnumerable<SidebarItem> GetSidebarItems()
        {
            yield return Instance.launchGroupsSidebarItem;
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

                logger.Debug($"{group.Name} - Trying to close {group.Action.Target}");

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