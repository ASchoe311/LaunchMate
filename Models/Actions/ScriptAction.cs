using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchMate.Models
{
    public class ScriptAction : ObservableObject, IAction
    {
        private readonly ILogger logger = LogManager.GetLogger();
        private string _target;
        private string _targetArgs;

        public string TargetUri { get => _target; set => SetValue(ref _target, value); }
        public string TargetArgs { get => _targetArgs; set => SetValue(ref _targetArgs, value); }

        void IAction.Execute()
        {
            logger.Debug($"Launching script \"{TargetUri}\" with arguments \"{TargetArgs}\"");
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/c " + TargetUri + " " + TargetArgs);
            Process.Start(TargetUri, TargetArgs);
            return;
        }
    }
}
