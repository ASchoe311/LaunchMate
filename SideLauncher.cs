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
using SideLauncher.Models;
using SideLauncher.Enums;
using SideLauncher.Views;
using SideLauncher.ViewModels;
using System.Windows;

namespace SideLauncher
{
    public class SideLauncher : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private SettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("61d7fcec-322d-4eb6-b981-1c8f8122ddc8");

        public SideLauncher(IPlayniteAPI api) : base(api)
        {
            settings = new SettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        private List<LaunchGroup> launchedGroups = new List<LaunchGroup>();

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
                AsyncLaunchGroups(args, group);
            }
        }

        private async void AsyncLaunchGroups(OnGameStartingEventArgs args, LaunchGroup group)
        {
            await Task.Run(() => HandleLaunchGroup(args, group));
        }

        private void HandleLaunchGroup(OnGameStartingEventArgs args, LaunchGroup group)
        {

            logger.Debug($"Checking launch conditions for group with app {Path.GetFileName(group.AppExePath)}");
            
            if (group.ShouldLaunchApp(args.Game))
            {
                logger.Debug($"Waiting {group.LaunchDelay} ms to launch {group.AppExePath}");
                System.Threading.Thread.Sleep(group.LaunchDelay);
                logger.Debug($"Launching {group.AppExePath} with arguments {group.AppExeArgs}");
                Process.Start(group.AppExePath, group.AppExeArgs);
                launchedGroups.Add(group);
            }

        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            base.OnGameStopped(args);
            foreach (var group in launchedGroups)
            {
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
                        if (item.Path != null && item.Path.Contains(Path.GetDirectoryName(group.AppExePath)) && group.AutoClose)
                        {
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