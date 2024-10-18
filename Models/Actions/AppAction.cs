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

        public string TargetUri { get => _target; set => SetValue(ref _target, value); }
        public string TargetArgs { get => _args; set => SetValue(ref _args, value); }
        public string LnkName { get => _lnkName; set => SetValue(ref _lnkName, value); }

        public void Execute()
        {
            logger.Debug($"Launching \"{TargetUri}\" with arguments \"{TargetArgs}\"");
            Process.Start(TargetUri, TargetArgs);
            return;
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
                    if (item.Path != null && item.Path.Contains(Path.GetDirectoryName(TargetUri)))
                    {
                        logger.Debug($"Stopping process {item.Process.ProcessName}|{item.Process.Id} associated with {TargetUri}");
                        item.Process.Kill();
                    }
                }
            }
        }

    }
}
