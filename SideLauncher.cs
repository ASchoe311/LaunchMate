using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SideLauncher
{
    public class SideLauncher : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        private SideLauncherSettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("61d7fcec-322d-4eb6-b981-1c8f8122ddc8");

        public SideLauncher(IPlayniteAPI api) : base(api)
        {
            settings = new SideLauncherSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
            logger.Debug($"Starting {args.Game.Source}");
            base.OnGameStarting(args);
        }

        private async void StartSideAppsAsync(OnGameStartingEventArgs args)
        {
            await Task.Run(() => StartSideApps(args));
        }

        private void StartSideApps(OnGameStartingEventArgs args)
        {
            foreach (var group in settings.Settings.LaunchGroups)
            {

                switch (group.FilterType)
                {
                    case FilterTypes.Source:

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
            return new SideLauncherSettingsView();
        }
    }
}