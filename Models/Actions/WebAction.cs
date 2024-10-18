using LaunchMate.Utilities;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace LaunchMate.Models
{
    public class WebAction : ObservableObject, IAction
    {
        private readonly ILogger logger = LogManager.GetLogger();

        private string _target;
        private bool _useWebView = true;

        public string TargetUri { get => _target; set => SetValue(ref _target, value); }
        public bool UseWebView { get => _useWebView; set => SetValue(ref _useWebView, value); }

        public IWebView WebView { get; set; } = null;

        public void Execute()
        {
            logger.Debug($"Opening webpage \"{TargetUri}\"");
            if (!UseWebView)
            {
                Process.Start(TargetUri);
                return;
            }
            // Dispatch to main thread
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                // Create a web view
                var windowSize = WindowHelper.GetNearMaxWindow(TargetUri);
                logger.Debug($"Creating webview to display {TargetUri}");
                var webView = API.Instance.WebViews.CreateView(windowSize.Item1, windowSize.Item2, System.Windows.Media.Colors.Black);
                webView.Navigate("https://" + TargetUri);
                webView.Open();
                // Watch for page loading changed
                webView.LoadingChanged += async (s, e) =>
                {
                    var pageSource = await webView.GetPageSourceAsync();
                    // Get <title> tag of page
                    var pageTitle = Regex.Match(pageSource, ".*?<title>(.*?)</title>.*").Groups[1].Value;
                    if (pageTitle != null && pageTitle != string.Empty)
                    {
                        // Bring web view to foreground
                        logger.Debug("Trying to bring webview into foreground");
                        SetForegroundHelper.SetForeground(pageTitle);
                    }
                };
                WebView = webView;
            });
            return;
        }


        public void AutoClose()
        {
            logger.Debug($"Closing webview for {TargetUri}");
            WebView.Close();
            WebView.Dispose();
            WebView = null;
        }

    }
}
