using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaunchMate.Enums;
using Playnite.SDK;
using LaunchMate.Models;
using LaunchMate.Views;
using LaunchMate.Utilities;
using System.Security.Principal;
using System.Windows;

namespace LaunchMate.ViewModels
{
    public class ConditionGroupEditorViewModel : ObservableObject
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly Settings _settings;
        private ConditionGroup _group;

        public ConditionGroupEditorViewModel(Settings settings, ConditionGroup group)
        {
            _settings = settings;
            _group = group;
        }

        public ConditionGroup Group { get => _group; set => SetValue(ref _group, value); }

        public RelayCommand AddConditionCmd
        {
            get => new RelayCommand(() =>
            {
                Group.Conditions.Add(new LaunchCondition());
            });
        }

        public RelayCommand<LaunchCondition> RemoveConditionCmd
        {
            get => new RelayCommand<LaunchCondition>((c) =>
            {
                if (c != null)
                {
                    Group.Conditions.Remove(c);
                }
            });
        }

        public Dictionary<string, JoinType> JoinMethodsDict { get; } = new Dictionary<string, JoinType>()
        {
            { "AND", JoinType.And },
            { "OR", JoinType.Or },
            { "XOR", JoinType.Xor }
        };

        public static Dictionary<string, FilterTypes> FilterTypesDict { get; } = new Dictionary<string, FilterTypes>
        {
            { "All Games", FilterTypes.All },
            { "Name", FilterTypes.Name },
            { "Source", FilterTypes.Source },
            { "Developer", FilterTypes.Developers },
            { "Publisher", FilterTypes.Publishers },
            { "Category", FilterTypes.Categories },
            { "Genre", FilterTypes.Genres },
            { "Game ID", FilterTypes.Gameid },
            { "Feature", FilterTypes.Features },
            { "Tag", FilterTypes.Tags },
            { "Platform", FilterTypes.Platforms },
            { "Series", FilterTypes.Series }
        };

        public static Window GetWindow(Settings settings, ConditionGroup conditionGroup)
        {
            try
            {
                var viewModel = new ConditionGroupEditorViewModel(settings, conditionGroup);
                var conditionGroupEditorView = new ConditionGroupEditorView(conditionGroup.Conditions);
                var window = WindowHelper.CreateSizedWindow
                (
                    "Condition Group Editor", 800, 400
                );
                window.Content = conditionGroupEditorView;
                window.DataContext = viewModel;
                return window;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception occurred during initialization of edit window");
                return null;
            }
        }

        public RelayCommand<Window> SaveCommand
        {
            get => new RelayCommand<Window>((w) =>
            {
                if (Group.Conditions.Count == 0)
                {
                    API.Instance.Dialogs.ShowMessage(
                        "Conditions list cannot be empty", string.Empty,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                w.DialogResult = true;
                w.Close();
            });
        }
    }
}
