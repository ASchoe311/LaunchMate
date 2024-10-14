using LaunchMate.Models;
using LaunchMate.Utilities;
using LaunchMate.Views;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LaunchMate.ViewModels
{
    public class MatchedGamesViewModel : ObservableObject
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly Settings _settings;
        private LaunchGroup _group;

        public MatchedGamesViewModel(Settings settings, LaunchGroup group)
        {
            _settings = settings;
            _group = group;
        }

        public LaunchGroup Group { get => _group; }

        public static Window GetWindow(Settings settings, LaunchGroup launchGroup)
        {
            try
            {
                var viewModel = new MatchedGamesViewModel(settings, launchGroup);
                var matchedGamesView = new MatchedGamesView();
                var window = WindowHelper.CreateSizedWindow
                (
                    $"Matched Games ({launchGroup.MatchedGames.Count})", 350, 500
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
