using SideLauncher.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SideLauncher.Models
{
    public class LaunchGroup : ObservableObject
    {
        private string _exePath = string.Empty;
        private string _exeArgs = string.Empty;
        private string _lnkName = null;
        private bool _enabled = true;
        private bool _autoClose = true;
        private bool _ignoreCase = false;
        private int _delay = 0;
        private bool _makeActions = false;
        private JoinType _joinType = JoinType.And;

        public string AppExePath { get => _exePath; set => SetValue(ref _exePath, value); }
        public string AppExeArgs { get => _exeArgs; set => SetValue(ref _exeArgs, value); }
        public string LnkName { get => _lnkName; set => SetValue(ref _lnkName, value); }
        public bool Enabled { get => _enabled; set => SetValue(ref _enabled, value); }
        public bool AutoClose { get => _autoClose; set => SetValue(ref _autoClose, value); }
        public bool IgnoreCase { get => _ignoreCase; set => SetValue(ref _ignoreCase, value); }
        public int LaunchDelay { get => _delay; set => SetValue(ref _delay, value); }
        public bool MakeGameActions { get => _makeActions; set => SetValue(ref _makeActions, value); }
        public JoinType JoinMethod { get => _joinType; set => SetValue(ref _joinType, value); }
        public ObservableCollection<LaunchCondition> Conditions { get; set; } = new ObservableCollection<LaunchCondition>();
        public int ConditionCount { get => Conditions.Count; }
    }
}
