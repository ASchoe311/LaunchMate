using LaunchMate.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaunchMate.Models
{
    public interface IAction
    {
        string Target { get; set; }
        string TargetArgs { get; set; }
        Screen OpenScreen { get; set; }
        bool Execute(string groupName, Screen screen = null);
        void AutoClose(string groupName);

    }
}
