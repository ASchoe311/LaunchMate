using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaunchMate.Models
{
    public class StartServiceAction : ActionBase
    {
        public override bool Execute(string groupName, Screen screen = null)
        {
            ILogger logger = LogManager.GetLogger();
            try
            {
                logger.Info($"{groupName} - Trying to start service {Target}");
                var svc = new ServiceController(Target);
                var status = svc.Status;
                if (status != ServiceControllerStatus.Running)
                {
                    svc.Start();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{groupName} - Something went wrong trying to start service {Target}");
                API.Instance.Notifications.Add($"{groupName} - Error: {Target}", $"An error occurred when LaunchMate tried to start service {Target} from group {groupName}, see logs for more info", NotificationType.Error);
                return false;
            }
        }
    }
}
