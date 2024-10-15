using LaunchMate.Models;
using LaunchMate.Utilities;
using LaunchMate.Views;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LaunchMate.ViewModels
{
    public class MatchedGamesViewModel : ObservableObject
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private LaunchGroup _group;
        private ObservableCollection<Game> _matches;

        public MatchedGamesViewModel(LaunchGroup group)
        {
            _group = group;
        }

        public ObservableCollection<Game> Matches { get => _matches; set => SetValue(ref _matches, value); }

        public LaunchGroup Group { get => _group; }

        /// <summary>
        /// Creates a window to present the list of matched games
        /// </summary>
        /// <param name="launchGroup"><see cref="LaunchGroup"/> for which to see matches</param>
        /// <returns>A <see cref="Window"/> with Content=<see cref="MatchedGamesView"/> and DataContext=<see cref="MatchedGamesViewModel"/></returns>
        public static Window GetWindow(LaunchGroup launchGroup)
        {
            try
            {
                var viewModel = new MatchedGamesViewModel(launchGroup);
                viewModel.Matches = launchGroup.MatchedGames;
                if (viewModel.Matches == null)
                {
                    logger.Debug("Match scanning cancelled, returning null window");
                    return null;
                }
                var matchedGamesView = new MatchedGamesView();
                var window = WindowHelper.CreateSizedWindow
                (
                    $"Matched Games ({viewModel.Matches.Count})", 350, 500
                );
                window.Content = matchedGamesView;
                window.DataContext = viewModel;
                return window;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception occurred during initialization of edit window");
                return null;
            }
        }
    }
}
