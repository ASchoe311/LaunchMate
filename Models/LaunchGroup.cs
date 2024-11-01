using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using LaunchMate.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LaunchMate.Models
{
    public class LaunchGroup : ObservableObject
    {
        private string _name = string.Empty;
        private ActionBase _action;
        private ActionType _actionType;
        private bool _enabled = true;
        private bool _autoClose = true;
        private bool _ignoreCase = false;
        private int _delay = 0;
        private bool _makeActions = false;

        public string Name { get => _name; set => SetValue(ref _name, value); }
        public ActionBase Action { get => _action; set => SetValue(ref _action, value); }
        public ActionType ActionType { get => _actionType; set => SetValue(ref _actionType, value); }
        public bool Enabled { get => _enabled; set => SetValue(ref _enabled, value); }
        public bool AutoClose { get => _autoClose; set => SetValue(ref _autoClose, value); }
        public bool IgnoreCase { get => _ignoreCase; set => SetValue(ref _ignoreCase, value); }
        public int LaunchDelay { get => _delay; set => SetValue(ref _delay, value); }
        public bool MakeGameActions { get => _makeActions; set => SetValue(ref _makeActions, value); }
        //public ObservableCollection<ConditionGroup> ConditionGroups { get; set; } = new ObservableCollection<ConditionGroup>();
        public ObservableCollection<LaunchCondition> Conditions { get; set; } = new ObservableCollection<LaunchCondition>();

        [DontSerialize]
        public IWebView webView { get; set; } = null;


        public LaunchGroup()
        {
            _action = new AppAction();
            _actionType = ActionType.App;
        }

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

                LaunchMate.Cache.Clear();

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
                        if (MeetsConditions(game))
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
        public bool MeetsConditions(Game game)
        {
            if (Conditions.Count == 0)
            {
                return true;
            }

            List<bool> matches = new List<bool>();
            foreach (var condition in Conditions)
            {
                bool condMet = condition.IsMet(game);
                if (condition.Not)
                {
#if DEBUG
                    logger.Debug("Not flag set, negating result");
#endif
                    condMet = !condMet;
                }
                matches.Add(condMet);
            }

            bool execute = matches[0];

            for (int i = 0; i < matches.Count - 1; i++)
            {
#if DEBUG
                logger.Debug($"App will launch given matches up to condition numer {i + 1}? {execute}");
#endif
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
#if DEBUG
            logger.Debug($"App will launch given matches up to condition numer {matches.Count}? {execute}");
#endif
            return execute;
        }

        /// <summary>
        /// Returns a string representation of the conditions within the launch group
        /// </summary>
        [DontSerialize]
        public string ToFilterString { get
            {
                string filterStr = string.Empty;
                if (Conditions.Count == 0)
                {
                    return "No Conditions";
                }
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
            } }

        [DontSerialize]
        public string TargetDisplayName => Name;
    }
}
