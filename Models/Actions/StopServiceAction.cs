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
    public class StopServiceAction : ActionBase
    {
        public override bool Execute(string groupName, Screen screen = null)
        {
            ILogger logger = LogManager.GetLogger();
            using (var sc = new ServiceController(Target))
            {
                try
                {
                    logger.Info($"{groupName} - Trying to stop service {Target}");
                    var status = sc.Status;
                    if (status == ServiceControllerStatus.Running && sc.CanStop)
                    {
                        sc.Stop();
                    }
                    else
                    {
                        logger.Info($"{groupName} - Service {Target} not running or not able to be stopped, skipping");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"{groupName} - Something went wrong trying to stop service  {Target}");
                    API.Instance.Notifications.Add($"{groupName} - Error: {Target}", $"An error occurred when LaunchMate tried to stop service {Target} from group {groupName}, see logs for more info", NotificationType.Error);
                    return false;
                }
            }
        }
    }
}
