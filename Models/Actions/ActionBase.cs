using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchMate.Models
{
    public abstract class ActionBase : ObservableObject, IAction
    {
        private readonly ILogger logger = LogManager.GetLogger();
        private string _target;
        private string _args;
        private bool _useWebView;

        public string Target { get => _target; set => SetValue(ref _target, value); }
        public string TargetArgs { get => _args; set => SetValue(ref _args, value); }
        public bool UseWebView { get => _useWebView; set => SetValue(ref _useWebView, value); }


        public abstract bool Execute(string groupName);

        public virtual void AutoClose(string groupName)
        {
            logger.Debug($"{groupName} - AutoClose not supported for this action type");
        }

    }
}
