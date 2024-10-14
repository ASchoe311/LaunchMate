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
        private ConditionGroup _group;

        public ConditionGroupEditorViewModel(ConditionGroup group)
        {
            _group = group;
        }

        public ConditionGroup Group { get => _group; set => SetValue(ref _group, value); }

        /// <summary>
        /// Adds a <see cref="LaunchCondition"/> to the current <see cref="ConditionGroup"/> 
        /// </summary>
        public RelayCommand AddConditionCmd
        {
            get => new RelayCommand(() =>
            {
                Group.Conditions.Add(new LaunchCondition());
            });
        }

        /// <summary>
        /// Removes the selected <see cref="LaunchCondition"/> from the current <see cref="ConditionGroup"/> 
        /// </summary>
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

        /// <summary>
        /// Dictionary to convert frontend logic types string representation to <see cref="JoinType"/> enums
        /// </summary>
        public Dictionary<string, JoinType> JoinMethodsDict { get; } = new Dictionary<string, JoinType>()
        {
            { "AND", JoinType.And },
            { "OR", JoinType.Or },
            { "XOR", JoinType.Xor }
        };

        /// <summary>
        /// Dictionary to convert frontend filter types string representation to <see cref="FilterTypes"/> enums
        /// </summary>
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

        /// <summary>
        /// Creates the window for displaying the condition group editor
        /// </summary>
        /// <param name="conditionGroup"><see cref="ConditionGroup"/> to edit</param>
        /// <returns>A <see cref="Window"/> with Content=<see cref="ConditionGroupEditorView"/>(<paramref name="conditionGroup"/>) 
        /// and DataContext=<see cref="ConditionGroupEditorViewModel"/>(<paramref name="conditionGroup"/>)</returns>
        public static Window GetWindow(ConditionGroup conditionGroup)
        {
            try
            {
                var viewModel = new ConditionGroupEditorViewModel(conditionGroup);
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

        /// <summary>
        /// Saves the edited <see cref="ConditionGroup"/> and closes the window
        /// </summary>
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
