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
    public class CloseAction : ActionBase
    {
        public override bool Execute(string groupName)
        {
            ILogger logger = LogManager.GetLogger();
            if ((Target ?? "") == string.Empty) return false;
            try
            {
                string targName = Target.Replace(".exe", "");
                logger.Debug($"{groupName} - Trying to close any running process by name {targName}");
                var procs = Process.GetProcessesByName(targName);
                foreach (var proc in procs)
                {
                    logger.Debug($"{groupName} - Killing process {proc.ProcessName}|{proc.Id}");
                    proc.Kill();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{groupName} - Something went wrong trying to close applications with name {Target}");
                API.Instance.Notifications.Add($"{groupName} - Error: {Target}", $"An error occurred when LaunchMate tried to close applications with name {Target} from group {groupName}, see logs for more info", NotificationType.Error);
                return false;
            }
        }
    }
}
