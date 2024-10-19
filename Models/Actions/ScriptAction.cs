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

        public string Target { get => _target; set => SetValue(ref _target, value); }
        public string TargetArgs { get => _targetArgs; set => SetValue(ref _targetArgs, value); }

        public bool Execute()
        {
            logger.Debug($"Launching script \"{Target}\" with arguments \"{TargetArgs}\"");
            try
            {
                API.Instance.Notifications.Remove($"Error - {Target}");
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/c " + Target + " " + TargetArgs)
                {
                    CreateNoWindow = true,
                    UseShellExecute = true
                };
                Process.Start(Target, TargetArgs);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Something went wrong trying to run script {Target}");
                API.Instance.Notifications.Add($"Error - {Target}", $"An error occurred when LaunchMate tried to run script {Target}, see logs for more info", NotificationType.Error);
                return false;
            }
        }
    }
}
