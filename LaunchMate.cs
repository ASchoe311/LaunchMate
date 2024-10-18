using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using LaunchMate.Models;
using LaunchMate.Enums;
using LaunchMate.Views;
using LaunchMate.ViewModels;
using System.Windows;
using LaunchMate.Utilities;

namespace LaunchMate
{
    public class LaunchMate : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private SettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("61d7fcec-322d-4eb6-b981-1c8f8122ddc8");

        private readonly int vNum = 1;

        public LaunchMate(IPlayniteAPI api) : base(api)
        {
            settings = new SettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        private Stack<LaunchGroup> toClose = new Stack<LaunchGroup>();

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            base.OnGameStarting(args);

            // Dispatch group handling to async function
            foreach (var group in settings.Settings.Groups)
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

            logger.Debug($"Checking launch group conditions for group names {group.Name}");
            
            if (group.MeetsConditions(args.Game))
            {
                logger.Debug($"Conditions passed, waiting {group.LaunchDelay} ms to execute action for group \"{group.Name}\"");
                System.Threading.Thread.Sleep(group.LaunchDelay);
                group.Action.Execute();
                if (group.AutoClose){
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

                switch (group.ActionType)
                {
                    case ActionType.App:
                        AppAction appAct = group.Action as AppAction;
                        appAct.AutoClose();
                        break;
                    case ActionType.Web:
                        WebAction webAct = group.Action as WebAction;
                        webAct.AutoClose();
                        break;
                    default:
                        break;
                }
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