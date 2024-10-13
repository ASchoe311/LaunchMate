using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using SideLauncher.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public ObservableCollection<ConditionGroup> ConditionGroups { get; set; } = new ObservableCollection<ConditionGroup>();

        [DontSerialize]
        private readonly ILogger logger = LogManager.GetLogger();

        [DontSerialize]
        public bool ShouldLaunchApp(Game game)
        {
            List<bool> matches = new List<bool>();
            foreach (var conditionGroup in ConditionGroups)
            {
                bool condMet = conditionGroup.IsMet(game);
                if (conditionGroup.Not)
                {
                    logger.Debug("Not flag set, negating result");
                    condMet = !condMet;
                }
                matches.Add(condMet);
            }

            bool execute = matches.Count == 0 ? true : matches[0];

            for (int i = 0; i < matches.Count - 1; i++)
            {
                logger.Debug($"App will launch given matches up to condition group number {i + 1}? {execute}");
                switch (ConditionGroups[i].Joiner)
                {
                    case JoinType.And:
                        execute &= matches[i + 1];
                        break;
                    case JoinType.Or:
                        execute |= matches[i + 1];
                        break;
                    case JoinType.Xor:
                        execute ^= matches[i + 1];
                        break;
                }
            }
            logger.Debug($"App will launch given matches up to condition group number {matches.Count}? {execute}");
            return execute;
        }

        [DontSerialize]
        public string ToFilterString { get
            {
                string filterStr = "Conditions: ";

                for (int i = 0; i < ConditionGroups.Count; i++)
                {
                    if (ConditionGroups[i].Not)
                    {
                        filterStr += "NOT ";
                    }
                    filterStr += "(";
                    for (int j = 0; j < ConditionGroups[i].Conditions.Count; j++)
                    {
                        var condition = ConditionGroups[i].Conditions[j];
                        filterStr += "(";
                        filterStr += $"\"{condition.FilterType}\" {(condition.Not ? "NOT" : "->")} \"{condition.Filter}\"";
                        filterStr += ")";
                        if (j < ConditionGroups[i].Conditions.Count - 1)
                        {
                            switch (condition.Joiner)
                            {
                                case JoinType.And:
                                    filterStr += " AND ";
                                    break;
                                case JoinType.Or:
                                    filterStr += " OR ";
                                    break;
                                case JoinType.Xor:
                                    filterStr += " XOR ";
                                    break;
                            }
                        }
                    }
                    filterStr += ")";
                    if (i < ConditionGroups.Count - 1)
                    {
                        switch (ConditionGroups[i].Joiner)
                        {
                            case JoinType.And:
                                filterStr += " AND ";
                                break;
                            case JoinType.Or:
                                filterStr += " OR ";
                                break;
                            case JoinType.Xor:
                                filterStr += " XOR ";
                                break;
                        }
                    }
                }
                return filterStr;
            } }

        [DontSerialize]
        public string AppDisplayName => LnkName != null ? LnkName : Path.GetFileName(AppExePath);

    }
}
