﻿using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using LaunchMate.Enums;
using LaunchMate.Utilities;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.IO;
using System.ServiceProcess;
using System.Text;

namespace LaunchMate.Models
{
    public class LaunchCondition : ObservableObject
    {
        private FilterTypes _filterType = FilterTypes.All;
        private string _condition = string.Empty;
        private bool _fuzzyMatch = false;
        private JoinType _joiner = JoinType.And;
        private bool _not = false;
        private Guid? _filterId;

        public FilterTypes FilterType { get => _filterType; set => SetValue(ref _filterType, value); } 
        public string Filter { get => _condition; set => SetValue(ref _condition, value); }
        public bool FuzzyMatch { get => _fuzzyMatch; set => SetValue(ref _fuzzyMatch, value); }
        public JoinType Joiner { get => _joiner; set => SetValue(ref _joiner, value); }
        public bool Not { get => _not; set => SetValue(ref _not, value); }
        public Guid? FilterId { get => _filterId; set => SetValue(ref _filterId, value); }


        [DontSerialize]
        private readonly ILogger logger = LogManager.GetLogger();

        public LaunchCondition()
        {
            FilterType = FilterTypes.All;
            Filter = string.Empty;
            FuzzyMatch = false;
        }

        /// <summary>
        /// Checks if the <see cref="LaunchCondition"/> is true for a given <see cref="Game"/>
        /// </summary>
        /// <param name="game"><see cref="Game"/> object to check against the <see cref="LaunchCondition"/></param>
        /// <returns>True if the <see cref="Game"/> matches the <see cref="LaunchCondition"/>, false otherwise</returns>
        [DontSerialize]
        public bool IsMet(Game game)
        {
            //RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
            //string pattern = $@"{Filter}";
            //StringBuilder stringBuilder = new StringBuilder();
            //foreach (char c in pattern)
            //{
            //    if (c == '\\')
            //    {
            //        stringBuilder.Append('\\');
            //    }
            //    stringBuilder.Append(c);
            //}
            //pattern = stringBuilder.ToString();
            //Regex rgx = new Regex(pattern, regexOptions);
            FilterPresetSettings filterSettings = new FilterPresetSettings()
            {
                UseAndFilteringStyle = true
            };
            bool pnMatch;
            //bool rgxMatch;
            switch (FilterType)
            {
                case FilterTypes.All:
                    return true;
                case FilterTypes.Name:
                    filterSettings.Name = Filter;
                    break;
                case FilterTypes.Source:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Source = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Source = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.Developers:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Developer = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Developer = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.Publishers:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Publisher = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Publisher = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.Categories:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Category = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Category = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.Genres:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Genre = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Genre = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                //case FilterTypes.GameId:
                //    //rgxMatch = rgx.IsMatch(game.GameId);
                //    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - N/A");
                //    if (game.GameId == Filter)
                //    {
                //        return true;
                //    }
                //    break;
                case FilterTypes.InstallDirectory:
                    //rgxMatch = rgx.IsMatch(game.InstallDirectory);
#if DEBUG
                    logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\":\nPlaynite Match - N/A");
#endif
                    if (game.InstallationStatus == InstallationStatus.Installed && Path.GetFullPath(game.InstallDirectory) == Path.GetFullPath(Filter))
                    {
                        return true;
                    }
                    return false;
                case FilterTypes.Tags:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Tag = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Tag = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.Features:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Feature = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Feature = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.AgeRatings:
                    if (FilterId.HasValue)
                    {
                        filterSettings.AgeRating = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.AgeRating = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.Series:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Series = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Series = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.Platforms:
                    if (FilterId.HasValue)
                    {
                        filterSettings.Platform = new IdItemFilterItemProperties(FilterId.Value);
                    }
                    else
                    {
                        filterSettings.Platform = new IdItemFilterItemProperties(Filter);
                    }
                    break;
                case FilterTypes.ExeName:


                    // Cache retrieval attempt
                    bool? isERunning = LaunchMate.Cache.ExeCache.IsRunning(Filter);
                    if (isERunning.HasValue)
                    {
                        return isERunning.Value;
                    }

                    // Cache miss
                    var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
                    using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                    using (var results = searcher.Get())
                    {
                        var query = from p in Process.GetProcesses()
                                    join mo in results.Cast<ManagementObject>()
                                    on p.Id equals (int)(uint)mo["ProcessId"]
                                    select new
                                    {
                                        Process = p,
                                        Path = (string)mo["ExecutablePath"],
                                        CommandLine = (string)mo["CommandLine"],
                                    };
                        foreach (var item in query)
                        {
                            if (item.Path != null && item.Path.ToLowerInvariant().MatchesPath(Filter.ToLowerInvariant()))
                            {
                                LaunchMate.Cache.ExeCache.SetRunning(Filter, true);
                                return true;
                            }
                        }
                    }
                        LaunchMate.Cache.ExeCache.SetRunning(Filter, false);
                    return false;
                case FilterTypes.Process:
#if DEBUG
                    logger.Debug($"Checking processes for {Filter}");
#endif

                    bool? isPRunning = LaunchMate.Cache.ProcessCache.IsRunning(Filter);
                    if (isPRunning.HasValue)
                    {
                        return isPRunning.Value;
                    }

                    foreach (var proc in Process.GetProcesses())
                    {
                        if (proc.ProcessName.ToLowerInvariant() == Filter.ToLowerInvariant())
                        {
#if DEBUG
                            logger.Debug($"{proc.ProcessName} matches filter {Filter}");
#endif
                            LaunchMate.Cache.ProcessCache.SetRunning(Filter, true);
                            return true;
                        }
                    }
                    LaunchMate.Cache.ProcessCache.SetRunning(Filter, false);
                    return false;
                case FilterTypes.Service:
#if DEBUG
                    logger.Debug($"Checking if service {Filter} is running");
#endif

                    bool? isSRunning = LaunchMate.Cache.ServiceCache.IsRunning(Filter);
                    if (isSRunning.HasValue)
                    {
                        return isSRunning.Value;
                    }

                    using (var sc = new ServiceController(Filter))
                    {
                        try
                        {
                            bool running = sc.Status == ServiceControllerStatus.Running;
#if DEBUG
                            logger.Debug($"{Filter} is a real service");
#endif
                            LaunchMate.Cache.ServiceCache.SetRunning(Filter, running);
                            return running;
                        }
                        catch
                        {
#if DEBUG
                            logger.Debug($"{Filter} is not a real service");
#endif
                            LaunchMate.Cache.ServiceCache.SetRunning(Filter, false);
                            return false;
                        }
                    }
                default:
                    return false;
            }
            pnMatch = API.Instance.Database.GetGameMatchesFilter(game, filterSettings, FuzzyMatch);
            //rgxMatch = rgx.IsMatchList(game.Platforms);
#if DEBUG
            logger.Debug($"Filter \"{Filter}\" on target \"{Enum.GetName(typeof(FilterTypes), FilterType)}\" for game \"{game.Name}\": Playnite Match - {pnMatch}");
#endif
            if (pnMatch)
            {
                return true;
            }
            return false;
        }

    }


}
