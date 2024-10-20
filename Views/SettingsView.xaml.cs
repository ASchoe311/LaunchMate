using System;
using System.Collections.Generic;
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
        public SettingsView(LaunchMate plugin)
        {
            InitializeComponent();

            DataContext = new SettingsViewModel(plugin);

            var viewModel = DataContext as SettingsViewModel;
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

            private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

    }
}