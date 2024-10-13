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
using SideLauncher.Enums;
using SideLauncher.Utilities;
using System.Text.RegularExpressions;

namespace SideLauncher.Models
{
    public class LaunchCondition : ObservableObject
    {
        private FilterTypes _filterType = FilterTypes.All;
        private string _condition = string.Empty;
        private bool _fuzzyMatch = true;

        public FilterTypes FilterType { get => _filterType; set => SetValue(ref _filterType, value); } 
        public string Filter { get => _condition; set => SetValue(ref _condition, value); }
        public bool FuzzyMatch { get => _fuzzyMatch; set => SetValue(ref _fuzzyMatch, value); }

        public LaunchCondition()
        {
            FilterType = FilterTypes.All;
            Filter = string.Empty;
            FuzzyMatch = true;
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
            switch (FilterType)
            {
                case FilterTypes.All:
                    return true;
                case FilterTypes.Name:
                    filterSettings.Name = Filter;
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                        rgx.IsMatch(game.Name))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Source:
                    filterSettings.Source = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                        rgx.IsMatch(game.Source.Name))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Developers:
                    filterSettings.Developer = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                        rgx.IsMatchList(game.Developers))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Publishers:
                    filterSettings.Publisher = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                        rgx.IsMatchList(game.Publishers))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Categories:
                    filterSettings.Category = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                       rgx.IsMatchList(game.Categories))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Genres:
                    filterSettings.Genre = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                       rgx.IsMatchList(game.Genres))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Gameid:
                    if (rgx.IsMatch(game.GameId))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.InstallDirectory:
                    if (rgx.IsMatch(game.InstallDirectory))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Tags:
                    filterSettings.Tag = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                       rgx.IsMatchList(game.Tags))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Features:
                    filterSettings.Feature = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                       rgx.IsMatchList(game.Features))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.AgeRatings:
                    filterSettings.AgeRating = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                       rgx.IsMatchList(game.AgeRatings))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Series:
                    filterSettings.Series = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                       rgx.IsMatchList(game.Series))
                    {
                        return true;
                    }
                    break;
                case FilterTypes.Platforms:
                    filterSettings.Platform = new IdItemFilterItemProperties(Filter);
                    if (API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch) ||
                       rgx.IsMatchList(game.Platforms))
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
