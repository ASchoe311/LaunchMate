using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace LaunchMate.Models
{


    public partial class OldSettings
    {
        [JsonProperty("PluginVersion", NullValueHandling = NullValueHandling.Ignore)]
        public long? PluginVersion { get; set; }

        [JsonProperty("Groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<Group> Groups { get; set; }
    }

    public partial class Group
    {
        [JsonProperty("LaunchTargetUri", NullValueHandling = NullValueHandling.Ignore)]
        public string LaunchTargetUri { get; set; }

        [JsonProperty("AppExeArgs", NullValueHandling = NullValueHandling.Ignore)]
        public string AppExeArgs { get; set; }

        [JsonProperty("LnkName", NullValueHandling = NullValueHandling.Ignore)]
        public string LnkName { get; set; }

        [JsonProperty("Enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }

        [JsonProperty("AutoClose", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutoClose { get; set; }

        [JsonProperty("IgnoreCase", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnoreCase { get; set; }

        [JsonProperty("LaunchDelay", NullValueHandling = NullValueHandling.Ignore)]
        public int? LaunchDelay { get; set; }

        [JsonProperty("MakeGameActions", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MakeGameActions { get; set; }

        [JsonProperty("ConditionGroups", NullValueHandling = NullValueHandling.Ignore)]
        public List<ConditionGroup> ConditionGroups { get; set; }
    }

    public partial class ConditionGroupOld
    {
        [JsonProperty("Not", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Not { get; set; }

        [JsonProperty("Joiner", NullValueHandling = NullValueHandling.Ignore)]
        public Enums.JoinType Joiner { get; set; }

        [JsonProperty("Conditions", NullValueHandling = NullValueHandling.Ignore)]
        public List<Condition> Conditions { get; set; }
    }

    public partial class Condition
    {
        [JsonProperty("FilterType", NullValueHandling = NullValueHandling.Ignore)]
        public Enums.FilterTypes FilterType { get; set; }

        [JsonProperty("Filter", NullValueHandling = NullValueHandling.Ignore)]
        public string Filter { get; set; }

        [JsonProperty("FuzzyMatch", NullValueHandling = NullValueHandling.Ignore)]
        public bool? FuzzyMatch { get; set; }

        [JsonProperty("Joiner", NullValueHandling = NullValueHandling.Ignore)]
        public Enums.JoinType Joiner { get; set; }

        [JsonProperty("Not", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Not { get; set; }
    }
}

