using LaunchMate.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaunchMate.Models
{
    public interface IAction
    {
        string Target { get; set; }
        string TargetArgs { get; set; }
        bool Execute(string groupName);
        void AutoClose(string groupName);

    }
}
