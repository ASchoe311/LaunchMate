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
        string TargetUri { get; set; }
        void Execute();
    }
}
