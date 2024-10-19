using LaunchMate.Enums;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchMate.Models
{
    public class CloseAction : ObservableObject, IAction
    {
        private string _target;
        private string _targetArgs = null;

        public string Target { get => _target; set => SetValue(ref _target, value); }
        public string TargetArgs { get => _targetArgs; set => SetValue(ref _targetArgs, value); }

        public bool Execute()
        {
            ILogger logger = LogManager.GetLogger();
            string targName = Target.Replace(".exe", "");
            logger.Debug($"Trying to close any running process by name {targName}");
            var procs = Process.GetProcessesByName(targName);
            foreach (var proc in procs)
            {
                logger.Debug($"Killing process {proc.ProcessName} - {proc.Id}");
                proc.Kill();
            }
            return true;
        }
    }
}
