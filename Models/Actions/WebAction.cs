﻿using LaunchMate.Utilities;
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
    public class WebAction : ActionBase
    {
        private readonly ILogger logger = LogManager.GetLogger();

        private bool _useWebView = true;
        public bool UseWebView { get => _useWebView; set => SetValue(ref _useWebView, value); }

        [DontSerialize]
        public IWebView WebView { get; set; } = null;

        public override bool Execute(string groupName)
        {
            try
            {
                logger.Debug($"{groupName} - Opening webpage \"{Target}\"");
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
                    logger.Debug($"{groupName} - Creating webview to display {Target}");
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
                            logger.Debug($"{groupName} - Trying to bring webview into foreground");
                            SetForegroundHelper.SetForeground(pageTitle);
                        }
                    };
                    WebView = webView;
                });
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"{groupName} - Something went wrong trying to open webpage {Target}");
                API.Instance.Notifications.Add($"{groupName} - Error: {Target}", $"An error occurred when LaunchMate tried to open webpage {Target} from group {groupName}, see logs for more info", NotificationType.Error);
                return false; 
            }
        }


        public override void AutoClose(string groupName)
        {
            logger.Debug($"{groupName} - Closing webview for {Target}");
            WebView.Close();
            WebView.Dispose();
            WebView = null;
        }

    }
}
