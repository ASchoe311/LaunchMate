using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace LaunchMate.Models
{
    public class Settings : ObservableObject
    {
        private int _pluginVersion = 2;
        private bool _usePlayniteWebview = true;
        private Screen _openOnScreen = Screen.PrimaryScreen;
        private bool _advancedMode = false;
        private bool _showFirstRunMessage = true;
        public int PluginVersion { get => _pluginVersion ; set => SetValue(ref _pluginVersion, value); }
        public bool UsePlayniteWebview { get => _usePlayniteWebview; set => SetValue(ref _usePlayniteWebview, value); }
        public ObservableCollection<LaunchGroup> Groups { get; set; } = new ObservableCollection<LaunchGroup>();
        public Screen OpenOnScreen { get => _openOnScreen; set => SetValue(ref _openOnScreen, value); }
        public bool AdvancedMode { get => _advancedMode; set => SetValue(ref _advancedMode, value); }
        public bool ShowFirstRunMessage { get => _showFirstRunMessage; set => SetValue(ref _showFirstRunMessage, value); }
    }
}
