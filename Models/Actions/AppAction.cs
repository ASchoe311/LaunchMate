using LaunchMate.Enums;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LaunchMate.Models
{
    public class AppAction : ObservableObject, IAction
    {
        private readonly ILogger logger = LogManager.GetLogger();

        private string _target;
        private string _args;
        private string _lnkName;

        public string Target { get => _target; set => SetValue(ref _target, value); }
        public string TargetArgs { get => _args; set => SetValue(ref _args, value); }
        public string LnkName { get => _lnkName; set => SetValue(ref _lnkName, value); }

        public bool Execute()
        {
            logger.Debug($"Launching application \"{Target}\" with arguments \"{TargetArgs}\"");
            try
            {
                API.Instance.Notifications.Remove($"Error - {Target}");
                Process.Start(Target, TargetArgs);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Something went wrong trying to start application at {Target}");
                API.Instance.Notifications.Add($"Error - {Target}", $"An error occurred when LaunchMate tried to launch application {Target}, see logs for more info", NotificationType.Error);
                return false;
            }
        }

        public void AutoClose()
        {
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };
                foreach (var item in query)
                {
                    // Check if the executable path of the process is the launched executable and stop the process
                    if (item.Path != null && item.Path.Contains(Path.GetDirectoryName(Target)))
                    {
                        logger.Debug($"Stopping process {item.Process.ProcessName}|{item.Process.Id} associated with {Target}");
                        item.Process.Kill();
                    }
                }
            }
        }

    }
}
