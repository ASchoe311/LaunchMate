using Microsoft.Win32;
using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shell32;
using System.IO;
using System.Reflection;
using LaunchMate.Models;
using LaunchMate.Enums;
using System.Runtime;

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
        /// Command to open the editing window for the selected <see cref="LaunchGroup"/>
        /// </summary>
        public RelayCommand<object> EditLaunchGroupCmd
        {
            get => new RelayCommand<object>((grp) =>
            {
                if (grp == null)
                {
                    return;
                }
                var grpOriginal = (LaunchGroup)grp;
                var toEdit = Serialization.GetClone(grpOriginal);

                var window = LaunchGroupEditorViewModel.GetWindow(toEdit);

                if (window == null)
                {
                    return;
                }

                if (!(window.ShowDialog() ?? false))
                {
                    return;
                }

                Settings.Groups.Remove(grpOriginal);
                Settings.Groups.Add(toEdit);

            });
        }

        /// <summary>
        /// Command to remove the selected <see cref="LaunchGroup"/>
        /// </summary>
        public RelayCommand<LaunchGroup> RemoveLaunchGroupCmd
        {
            get => new RelayCommand<LaunchGroup>((a) =>
            {
                if (a == null) {  return; }
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

        public SettingsViewModel(LaunchMate plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<Settings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new Settings();
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
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}