using LaunchMate.Enums;
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
    public class CloseAction : ActionBase
    {
        public override bool Execute(string groupName, Screen screen = null)
        {
            ILogger logger = LogManager.GetLogger();
            if ((Target ?? "") == string.Empty)
            {
                return false;
            }
            try
            {
                //string targName = Target.Replace(".exe", "");
                logger.Info($"{groupName} - Trying to stop any running process by name {Target}");
                var procs = Process.GetProcessesByName(Target);
                foreach (var proc in procs)
                {
#if DEBUG
                    logger.Debug($"{groupName} - Killing process {proc.ProcessName}|{proc.Id}");
#endif
                    proc.Kill();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{groupName} - Something went wrong trying to stop processes with name {Target}");
                API.Instance.Notifications.Add($"{groupName} - Error: {Target}", $"An error occurred when LaunchMate tried to stop processes with name {Target} from group {groupName}, see logs for more info", NotificationType.Error);
                return false;
            }
        }
    }
}
