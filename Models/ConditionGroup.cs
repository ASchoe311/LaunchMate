﻿using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using LaunchMate.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LaunchMate.Models
{
    public class ConditionGroup : ObservableObject
    {
        private bool _not;
        private JoinType _joinType;

        [JsonProperty("Not", NullValueHandling = NullValueHandling.Ignore)]
        public bool Not { get => _not; set => SetValue(ref _not, value); }
        [JsonProperty("Joiner", NullValueHandling = NullValueHandling.Ignore)]
        public JoinType Joiner { get => _joinType; set => SetValue(ref _joinType, value); }
        [JsonProperty("Conditions", NullValueHandling = NullValueHandling.Ignore)]
        public ObservableCollection<LaunchCondition> Conditions { get; set; } = new ObservableCollection<LaunchCondition>();

        [DontSerialize]
        private readonly ILogger logger = LogManager.GetLogger();


        /// <summary>
        /// Returns a string representation of the conditions within the condition group
        /// </summary>
        [DontSerialize]
        public string ToFilterString
        {
            get
            {
                string filterStr = string.Empty;
                for (int j = 0; j < Conditions.Count; j++)
                {
                    var condition = Conditions[j];
                    filterStr += "(";
                    filterStr += $"\"{condition.FilterType}\" {(condition.Not ? (condition.FuzzyMatch ? (ResourceProvider.GetString("LOCLaunchMateNot") + "~") : ResourceProvider.GetString("LOCLaunchMateNot")) : (condition.FuzzyMatch ? "~>" : "->"))} \"{condition.Filter}\"";
                    filterStr += ")";
                    if (j < Conditions.Count - 1)
                    {
                        switch (condition.Joiner)
                        {
                            case JoinType.And:
                                filterStr += " " + ResourceProvider.GetString("LOCLaunchMateAnd") + " ";
                                break;
                            case JoinType.Or:
                                filterStr += " " + ResourceProvider.GetString("LOCLaunchMateOr") + " ";
                                break;
                            case JoinType.Xor:
                                filterStr += " " + ResourceProvider.GetString("LOCLaunchMateXor") + " ";
                                break;
                        }
                    }
                }
                return filterStr;
            }
        }

        /// <summary>
        /// Checks if <see cref="LaunchCondition.IsMet(Game)"/> is true for all conditions within the <see cref="ConditionGroup"/> for the given <see cref="Game"/>
        /// </summary>
        /// <param name="game"><see cref="Game"/> object to check against the conditions within the <see cref="ConditionGroup"/></param>
        /// <returns>True if all conditions are met, false otherwise</returns>
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
