using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaunchMate.Models
{
    public class ScriptAction : ActionBase
    {
        private readonly ILogger logger = LogManager.GetLogger();

        public override bool Execute(string groupName, Screen screen = null)
        {
            if ((Target ?? "") == string.Empty) return false;
            logger.Debug($"{groupName} - Launching script \"{Target}\" with arguments \"{TargetArgs}\"");
            try
            {
                API.Instance.Notifications.Remove($"{groupName}  - Error: {Target}");
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
                API.Instance.Notifications.Add($"{groupName}  - Error: {Target}", $"An error occurred when LaunchMate tried to run script {Target} from group {groupName}, see logs for more info", NotificationType.Error);
                return false;
            }
        }
    }
}
