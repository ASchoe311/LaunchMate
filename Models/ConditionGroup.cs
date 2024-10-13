using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using SideLauncher.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SideLauncher.Models
{
    public class ConditionGroup : ObservableObject
    {
        private bool _not;
        private JoinType _joinType;

        public bool Not { get => _not; set => SetValue(ref _not, value); }
        public JoinType Joiner { get => _joinType; set => SetValue(ref _joinType, value); }
        public ObservableCollection<LaunchCondition> Conditions { get; set; } = new ObservableCollection<LaunchCondition>();

        [DontSerialize]
        private readonly ILogger logger = LogManager.GetLogger();

        [DontSerialize]
        public string ToFilterString
        {
            get
            {
                string filterStr = "Conditions: ";
                for (int j = 0; j < Conditions.Count; j++)
                {
                    var condition = Conditions[j];
                    filterStr += "(";
                    filterStr += $"\"{condition.FilterType}\" {(condition.Not ? "NOT" : "->")} \"{condition.Filter}\"";
                    filterStr += ")";
                    if (j < Conditions.Count - 1)
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
                return filterStr;
            }
        }

        public bool IsMet(Game game)
        {
            List<bool> matches = new List<bool>();
            foreach (var condition in Conditions)
            {
                bool condMet = condition.IsMet(game);
                if (condition.Not)
                {
                    logger.Debug("Not flag set, negating result");
                    condMet = !condMet;
                }
                matches.Add(condMet);
            }

            bool execute = matches[0];

            for (int i = 0; i < matches.Count - 1; i++)
            {
                logger.Debug($"App will launch given matches up to condition numer {i + 1}? {execute}");
                switch (Conditions[i].Joiner)
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
            return execute;
        }
    }
}
