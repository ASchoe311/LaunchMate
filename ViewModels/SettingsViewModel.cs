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
        private Settings editingClone { get; set; }

        private Settings settings;
        public Settings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand AddLaunchGroupCmd
        {
            get => new RelayCommand(() =>
            {
                var launchGroup = new LaunchGroup();

                var window = LaunchGroupEditorViewModel.GetWindow(Settings, launchGroup);

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

                var window = LaunchGroupEditorViewModel.GetWindow(Settings, toEdit);

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

        public RelayCommand<LaunchGroup> RemoveLaunchGroupCmd
        {
            get => new RelayCommand<LaunchGroup>((a) =>
            {
                if (a == null) {  return; }
                Settings.Groups.Remove(a);
            });
        }

        public RelayCommand<LaunchGroup> ShowMatchesCmd
        {
            get => new RelayCommand<LaunchGroup>((grp) =>
            {

                if (grp == null)
                {
                    return;
                }
                var window = MatchedGamesViewModel.GetWindow(Settings, grp);
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
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
            //plugin.UpdateGameActions();
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