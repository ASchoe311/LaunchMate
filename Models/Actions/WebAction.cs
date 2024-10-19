using LaunchMate.Utilities;
using Playnite.SDK;
using Playnite.SDK.Data;
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
        private string _targetArgs = null;

        public string Target { get => _target; set => SetValue(ref _target, value); }
        public bool UseWebView { get => _useWebView; set => SetValue(ref _useWebView, value); }
        public string TargetArgs { get => _targetArgs; set => SetValue(ref _targetArgs, value); }

        [DontSerialize]
        public IWebView WebView { get; set; } = null;

        public bool Execute()
        {
            logger.Debug($"Opening webpage \"{Target}\"");
            if (!UseWebView)
            {
                Process.Start(Target);
                return false;
            }
            // Dispatch to main thread
            API.Instance.MainView.UIDispatcher.Invoke((Action)delegate
            {
                // Create a web view
                var windowSize = WindowHelper.GetNearMaxWindow(Target);
                logger.Debug($"Creating webview to display {Target}");
                var webView = API.Instance.WebViews.CreateView(windowSize.Item1, windowSize.Item2, System.Windows.Media.Colors.Black);
                webView.Navigate("https://" + Target);
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
            return true;
        }


        public void AutoClose()
        {
            logger.Debug($"Closing webview for {Target}");
            WebView.Close();
            WebView.Dispose();
            WebView = null;
        }

    }
}
