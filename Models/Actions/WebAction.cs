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
                    webView.Open();
                    webView.Navigate(Target);

                    // Watch for page loading changed
                    // Not using for now, has some issue that causes extreme CPU usage
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


                    //BringPageForward(webView, groupName);


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

        //private async void BringPageForward(IWebView webView, string groupName)
        //{
        //    await Task.Run(() =>
        //    {
        //        int cycles = 0;

        //        while (true)
        //        {
        //            var pageSource = webView.GetPageSource();
        //            var pageTitle = Regex.Match(pageSource, ".*?<title>(.*?)</title>.*").Groups[1].Value;
        //            if (pageTitle != null && pageTitle != string.Empty)
        //            {
        //                // Bring web view to foreground
        //                logger.Debug($"{groupName} - Trying to bring webview into foreground");
        //                SetForegroundHelper.SetForeground(pageTitle);
        //                break;
        //            }

        //            if (cycles >= 25)
        //            {
        //                logger.Debug($"{groupName} - Awaiting webview took too long, leaving in background");
        //                break;
        //            }

        //            System.Threading.Thread.Sleep(100);
        //        }
        //    });
        //}

        public override void AutoClose(string groupName)
        {
            logger.Debug($"{groupName} - Closing webview for {Target}");
            WebView.Close();
            WebView.Dispose();
            WebView = null;
        }

    }
}