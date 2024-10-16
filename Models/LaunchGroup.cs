using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using LaunchMate.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LaunchMate.Utilities;

namespace LaunchMate.Models
{
    public class LaunchGroup : ObservableObject
    {
        private string _targetUri = string.Empty;
        private string _exeArgs = string.Empty;
        private string _lnkName = null;
        private bool _enabled = true;
        private bool _autoClose = true;
        private bool _ignoreCase = false;
        private int _delay = 0;
        private bool _makeActions = false;

        public string LaunchTargetUri { get => _targetUri; set => SetValue(ref _targetUri, value); }
        public string AppExeArgs { get => _exeArgs; set => SetValue(ref _exeArgs, value); }
        public string LnkName { get => _lnkName; set => SetValue(ref _lnkName, value); }
        public bool Enabled { get => _enabled; set => SetValue(ref _enabled, value); }
        public bool AutoClose { get => _autoClose; set => SetValue(ref _autoClose, value); }
        public bool IgnoreCase { get => _ignoreCase; set => SetValue(ref _ignoreCase, value); }
        public int LaunchDelay { get => _delay; set => SetValue(ref _delay, value); }
        public bool MakeGameActions { get => _makeActions; set => SetValue(ref _makeActions, value); }
        public ObservableCollection<ConditionGroup> ConditionGroups { get; set; } = new ObservableCollection<ConditionGroup>();

        [DontSerialize]
        public IWebView webView { get; set; } = null;


        [DontSerialize]
        private readonly ILogger logger = LogManager.GetLogger();

        /// <summary>
        /// Returns a collection of games for which this launch group is a match
        /// Shows a progress bar while scanning and allows cancellation
        /// </summary>
        [DontSerialize]
        public ObservableCollection<Game> MatchedGames { get
            {
                var matches = new ObservableCollection<Game>();
                int numGames = API.Instance.Database.Games.Count;
                var gpo = new GlobalProgressOptions(ResourceProvider.GetString("LOCLaunchMateCheckingGames"), true);
                gpo.IsIndeterminate = false;
                bool cancelled = false;
                API.Instance.Dialogs.ActivateGlobalProgress((activateGlobalProgress) =>
                {
                    activateGlobalProgress.ProgressMaxValue = numGames;
                    activateGlobalProgress.CurrentProgressValue = -1;
                    foreach (var game in API.Instance.Database.Games)
                    {
                        if (activateGlobalProgress.CancelToken.IsCancellationRequested)
                        {
                            cancelled = true;
                            break;
                        }
                        activateGlobalProgress.CurrentProgressValue += 1;
                        if (ShouldLaunchApp(game))
                        {
                            matches.Add(game);
                        }
                    }
                }, gpo);

                return cancelled ? null : matches;
            }
        } 

        /// <summary>
        /// Determines whether or not the executable in the <see cref="LaunchGroup"/> should launch for the given <see cref="Game"/>
        /// by checking <see cref="ConditionGroup.IsMet(Game)"/> for each <see cref="ConditionGroup"/> in the <see cref="LaunchGroup"/>
        /// </summary>
        /// <param name="game"><see cref="Game"/> object to check against</param>
        /// <returns>True if launch conditions evaluate to true, false otherwise</returns>
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

        /// <summary>
        /// Returns a string representation of the conditions within the launch group
        /// </summary>
        [DontSerialize]
        public string ToFilterString { get
            {
                string filterStr = string.Empty;

                for (int i = 0; i < ConditionGroups.Count; i++)
                {
                    if (ConditionGroups[i].Not)
                    {
                        filterStr += ResourceProvider.GetString("LOCLaunchMateNot") + " ";
                    }
                    filterStr += "(";
                    filterStr += ConditionGroups[i].ToFilterString;
                    filterStr += ")";
                    if (i < ConditionGroups.Count - 1)
                    {
                        switch (ConditionGroups[i].Joiner)
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
            } }

        [DontSerialize]
        public string TargetDisplayName => LnkName ?? Path.GetFileName(LaunchTargetUri);
    }
}
