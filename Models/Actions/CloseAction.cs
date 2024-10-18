using LaunchMate.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchMate.Models
{
    public class CloseAction : ObservableObject, IAction
    {
        private string _target;

        public string TargetUri { get => _target; set => SetValue(ref _target, value); }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
