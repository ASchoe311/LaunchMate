using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchMate.Models
{
    public class Settings : ObservableObject
    {
        private int _pluginVersion = 1;
        private bool _usePlayniteWebview = true;
        public int PluginVersion { get => _pluginVersion ; set => SetValue(ref _pluginVersion, value); }
        public bool UsePlayniteWebview { get => _usePlayniteWebview; set => SetValue(ref _usePlayniteWebview, value); }
        public ObservableCollection<LaunchGroup> Groups { get; set; } = new ObservableCollection<LaunchGroup>();
    }
}
