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
using System.Windows.Forms;

namespace LaunchMate.Models
{
    public class WebAction : ActionBase
    {
        private readonly ILogger logger = LogManager.GetLogger();

        [DontSerialize]
        public IWebView WebView { get; set; } = null;

        public override bool Execute(string groupName, Screen screen = null)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }
            if ((Target ?? "") == string.Empty) return false;
            try
            {
                logger.Info($"{groupName} - Opening webpage \"{Target}\"");
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
#if DEBUG
                    logger.Debug($"{groupName} - Creating webview to display {Target}");
#endif
                    var webView = API.Instance.WebViews.CreateView(windowSize.Item1, windowSize.Item2, System.Windows.Media.Colors.Black);
                    webView.Open();
                    webView.Navigate(Target);

                    // Watch for page loading changed
                    // Potential for high CPU usage for certain pages. Uncertain of cause
                    // Only known example so far: atlauncher.com
                    webView.LoadingChanged += async (s, e) =>
                    {
                        var pageSource = await webView.GetPageSourceAsync();
                        // Get <title> tag of page
                        var pageTitle = Regex.Match(pageSource, ".*?<title>(.*?)</title>.*").Groups[1].Value;
                        if (pageTitle != null && pageTitle != string.Empty)
                        {
                            // Bring web view to foreground
#if DEBUG
                            logger.Debug($"{groupName} - Trying to bring webview into foreground");
#endif
                            //WindowHelper.MoveWindow(pageTitle, screen);
                            WindowHelper.SetForeground(pageTitle);
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

        public override void AutoClose(string groupName)
        {
            logger.Info($"{groupName} - Closing webview for {Target}");
            WebView.Close();
            WebView.Dispose();
            WebView = null;
        }

    }
}
