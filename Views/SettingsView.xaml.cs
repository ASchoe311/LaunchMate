using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LaunchMate.Models;
using LaunchMate.ViewModels;
using LaunchMate.Views;
using Playnite.SDK.Controls;

namespace LaunchMate
{
    public partial class SettingsView : UserControl
    {
        private SettingsViewModel _viewModel;
        public SettingsView(LaunchMate plugin)
        {
            InitializeComponent();

            DataContext = new SettingsViewModel(plugin);

            var viewModel = DataContext as SettingsViewModel;
            _viewModel = viewModel;
            if (viewModel != null)
            {
                foreach (var launchGroup in viewModel.Settings.Groups)
                {
                    var tabItem = new TabItem
                    {
                        Header = launchGroup.Name,
                        Content = new LaunchGroupEditorView(launchGroup)
                    };

                    tabControl.Items.Add(tabItem);
                }

            }

        }

    }

}
