using LaunchMate.Enums;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchMate.Models
{
    public interface ICondition
    {
        FilterTypes FilterType { get; set; }
        string Filter { get; set; }
        JoinType Joiner { get; set; }
        bool Not {  get; set; }
        bool FuzzyMatch { get; set; }
        bool IsMet(Game game);
    }
}
