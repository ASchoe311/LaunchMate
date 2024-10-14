using Microsoft.Win32;
using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shell32;
using System.IO;
using LaunchMate.Enums;
using LaunchMate.Utilities;
using System.Text.RegularExpressions;

namespace LaunchMate.Models
{
    public class LaunchCondition : ObservableObject
    {
        private FilterTypes _filterType = FilterTypes.All;
        private string _condition = string.Empty;
        private bool _fuzzyMatch = false;
        private JoinType _joiner = JoinType.And;
        private bool _not = false;

        public FilterTypes FilterType { get => _filterType; set => SetValue(ref _filterType, value); } 
        public string Filter { get => _condition; set => SetValue(ref _condition, value); }
        public bool FuzzyMatch { get => _fuzzyMatch; set => SetValue(ref _fuzzyMatch, value); }
        public JoinType Joiner { get => _joiner; set => SetValue(ref _joiner, value); }
        public bool Not { get => _not; set => SetValue(ref _not, value); }


        [DontSerialize]
        private readonly ILogger logger = LogManager.GetLogger();

        public LaunchCondition()
        {
            FilterType = FilterTypes.All;
            Filter = string.Empty;
            FuzzyMatch = false;
        }

        [DontSerialize]
        public bool IsMet(Game game)
        {
            RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
            string pattern = $@"{Filter}";
            Regex rgx = new Regex(pattern, regexOptions);
            FilterPresetSettings filterSettings = new FilterPresetSettings()
            {
                UseAndFilteringStyle = true
            };
            bool pnMatch, rgxMatch;
            switch (FilterType)
            {
                case FilterTypes.All:
                    return true;
                case FilterTypes.Name:
                    filterSettings.Name = Filter;
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatch(game.Name);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Source:
                    filterSettings.Source = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatch(game.Source.Name);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Developers:
                    filterSettings.Developer = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatch(game.Name);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Publishers:
                    filterSettings.Publisher = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatchList(game.Publishers);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Categories:
                    filterSettings.Category = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatchList(game.Categories);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Genres:
                    filterSettings.Genre = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatchList(game.Genres);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Gameid:
                    rgxMatch = rgx.IsMatch(game.GameId);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - N/A  |  Regex Match - {rgxMatch}");
                    if (rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.InstallDirectory:
                    rgxMatch = rgx.IsMatch(game.InstallDirectory);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\":\nPlaynite Match - N/A  |  Regex Match - {rgxMatch}");
                    if (rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Tags:
                    filterSettings.Tag = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatchList(game.Tags);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Features:
                    filterSettings.Feature = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatchList(game.Features);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.AgeRatings:
                    filterSettings.AgeRating = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatchList(game.AgeRatings);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Series:
                    filterSettings.Series = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatchList(game.Series);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Platforms:
                    filterSettings.Platform = new IdItemFilterItemProperties(Filter);
                    pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
                    rgxMatch = rgx.IsMatchList(game.Platforms);
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}  |  Regex Match - {rgxMatch}");
                    if (pnMatch || rgxMatch)
                    {
                        return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

    }
}
