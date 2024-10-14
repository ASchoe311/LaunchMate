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

        private Stack<LaunchGroup> launchedGroups = new Stack<LaunchGroup>();

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

            logger.Debug($"Checking launch conditions for group with app {Path.GetFileName(group.AppExePath)}");
            
            if (group.ShouldLaunchApp(args.Game))
            {
                logger.Debug($"Waiting {group.LaunchDelay} ms to launch \"{group.AppExePath}\"");
                System.Threading.Thread.Sleep(group.LaunchDelay);
                logger.Debug($"Launching \"{group.AppExePath}\" with arguments \"{group.AppExeArgs}\"");
                Process.Start(group.AppExePath, group.AppExeArgs);
                launchedGroups.Push(group);
            }

        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            base.OnGameStopped(args);
            while (launchedGroups.Count > 0)
            {
                LaunchGroup group = launchedGroups.Pop();
                
                // Use WMI to get all processes and their executable paths
                var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
                using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                using (var results = searcher.Get())
                {
                    var query = from p in Process.GetProcesses()
                                join mo in results.Cast<ManagementObject>()
                                on p.Id equals (int)(uint)mo["ProcessId"]
                                select new
                                {
                                    Process = p,
                                    Path = (string)mo["ExecutablePath"],
                                    CommandLine = (string)mo["CommandLine"],
                                };
                    foreach (var item in query)
                    {
                        // Check if the executable path of the process is the launched executable and stop the process
                        if (item.Path != null && item.Path.Contains(Path.GetDirectoryName(group.AppExePath)) && group.AutoClose)
                        {
                            logger.Debug($"Stopping process {item.Process.ProcessName}|{item.Process.Id} associated with {group.AppDisplayName}");
                            item.Process.Kill();
                        }
                    }
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