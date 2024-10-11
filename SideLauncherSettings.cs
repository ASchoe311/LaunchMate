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

namespace SideLauncher
{
    public enum FilterTypes
    {
        All = 0,
        Name = 1,
        Source = 2,
        Developers = 3,
        Publishers = 4,
        Categories = 5,
        Roms = 6,
        Genres = 7,
        Gameid = 8,
        InstallDirectory = 9,
        SortingName = 10,
        PluginId = 11,
        Tags = 12,
        Features = 13,
        AgeRatings = 14,
        Series = 15,
        Platforms = 16,
    }

    public class LaunchGroup
    {
        public FilterTypes FilterType { get; set; } = FilterTypes.All;
        public string Filter { get; set; } = string.Empty;
        public string AppExePath { get; set; } = string.Empty;
        public string AppExeArgs { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public bool AutoClose { get; set; } = true;

        public LaunchGroup()
        {
            FilterType = FilterTypes.All;
            Filter = string.Empty;
            AppExePath = string.Empty;
            AppExeArgs = string.Empty;
            Enabled = true;
            AutoClose = true;
        }
    }

    public class SideLauncherSettings : ObservableObject
    {
        public bool IgnoreCase { get; set; } = true;
        public ObservableCollection<LaunchGroup> LaunchGroups { get; set; } = new ObservableCollection<LaunchGroup>();
    }

    public class SideLauncherSettingsViewModel : ObservableObject, ISettings
    {
        private readonly SideLauncher plugin;
        private SideLauncherSettings editingClone { get; set; }

        private SideLauncherSettings settings;
        public SideLauncherSettings Settings
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
                Tuple<string, string> app = SelectApp();
                if (app != null)
                {
                    LaunchGroup lg = new LaunchGroup();
                    lg.AppExePath = app.Item1;
                    lg.AppExeArgs = app.Item2;
                    Settings.LaunchGroups.Add(lg);
                }
            });
        }

        private Tuple<string, string> SelectApp()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DereferenceLinks = false;
            openFileDialog.Filter = "Executable file (.exe)|*.exe";


            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string lnkName = string.Empty;
                string targetname = openFileDialog.FileName;
                string args = string.Empty;
                if (openFileDialog.FileName.Contains(".lnk"))
                {
                    //logger.Debug("File chosen is a shortcut, trying to extract target and args");
                    // Open document
                    lnkName = openFileDialog.FileName;
                    string pathOnly = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                    string filenameOnly = System.IO.Path.GetFileName(openFileDialog.FileName);

                    Shell shell = new Shell();
                    Shell32.Folder folder = shell.NameSpace(pathOnly);
                    FolderItem folderItem = folder.ParseName(filenameOnly);
                    if (folderItem != null)
                    {
                        Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                        targetname = link.Target.Path;  // <-- main difference
                        args = link.Arguments;
                        if (targetname.StartsWith("{"))
                        { // it is prefixed with {54A35DE2-guid-for-program-files-x86-QZ32BP4}
                            int endguid = targetname.IndexOf("}");
                            if (endguid > 0)
                            {
                                targetname = "C:\\program files (x86)" + targetname.Substring(endguid + 1);
                            }
                        }
                        //string file = LnkToFile(openFileDialog.FileName);
                    }
                }
                return new Tuple<string, string>(targetname, args);
            }
            return null;
        }

        public RelayCommand<LaunchGroup> RemoveLaunchGroupCmd
        {
            get => new RelayCommand<LaunchGroup>((a) =>
            {
                if (a == null) {  return; }
                Settings.LaunchGroups.Remove(a);
            });
        }


        public static Dictionary<string, FilterTypes> FilterTypesDict { get; } = new Dictionary<string, FilterTypes>
        {
            { "All", FilterTypes.All },
            { "Name", FilterTypes.Name },
            { "Source", FilterTypes.Source },
            { "Developer", FilterTypes.Developers },
            { "Publisher", FilterTypes.Publishers },
            { "Category", FilterTypes.Categories },
            { "ROM", FilterTypes.Roms },
            { "Genre", FilterTypes.Genres },
            { "Game ID", FilterTypes.Gameid },
            { "Feature", FilterTypes.Features },
            { "Tag", FilterTypes.Tags },
            { "Platform", FilterTypes.Platforms },
            { "Series", FilterTypes.Series },
            { "Plugin ID", FilterTypes.PluginId }
        };

        public SideLauncherSettingsViewModel(SideLauncher plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<SideLauncherSettings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new SideLauncherSettings();
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