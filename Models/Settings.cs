using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SideLauncher.Models
{
    public class Settings : ObservableObject
    {
        public ObservableCollection<LaunchGroup> Groups { get; set; } = new ObservableCollection<LaunchGroup>();
    }
}
