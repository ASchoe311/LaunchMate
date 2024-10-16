using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace LaunchMate.Utilities
{

    /// <summary>
    /// Stack overflow magic from <see href="https://stackoverflow.com/a/2584672"/> and <see href="https://stackoverflow.com/a/49822416"/>
    /// </summary>
    public class SetForegroundHelper
    {
        public static void SetForeground(string windowTitle)
        {
            const uint WM_GETTEXT = 0x000D;
            foreach (var handle in SetForegroundHelper.EnumerateProcessWindowHandles(Process.GetProcessesByName("Playnite.DesktopApp").First().Id))
            {
                StringBuilder message = new StringBuilder(1000);
                SetForegroundHelper.SendMessage(handle, WM_GETTEXT, message.Capacity, message);
                if (message.ToString().Contains(windowTitle))
                {
                    SetForegroundHelper.SetForegroundWindow(handle);
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr handle);

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        public static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }
    }

    public static class WindowHelper
    {

        /// <summary>
        /// Get a width and a height near the max size of the user's screen
        /// </summary>
        /// <param name="title"></param>
        /// <returns>A Tuple<int, int> where item1 is the width and item2 is the height</returns>
        public static Tuple<int, int> GetNearMaxWindow(string title)
        {
            var window = CreateWindow(title);
            var ioHelper = new WindowInteropHelper(window.Owner);
            var hWnd = ioHelper.Handle;
            var screen = Screen.FromHandle(hWnd);
            var dpi = VisualTreeHelper.GetDpi(window);
            int width = (int)(screen.WorkingArea.Width * 0.96D / dpi.DpiScaleY);
            int height = (int)(screen.WorkingArea.Height * 0.96D / dpi.DpiScaleY);
            window.Close();
            return Tuple.Create(width, height);

        }

        /// <summary>
        /// Creates a <see cref="Window"/> with the given title, width, and height
        /// </summary>
        /// <param name="title">Title to give the window</param>
        /// <param name="width">Width of the new window</param>
        /// <param name="height">Height of the new window</param>
        /// <param name="widthToMax">Bool to set width to max screen width</param>
        /// <param name="heightToMax">Bool to set height to max screen height</param>
        /// <returns>A <see cref="Window"/> with the given attributes</returns>
        public static Window CreateSizedWindow(string title, int width, int height, bool widthToMax = false, bool heightToMax = false)
        {
            var window = CreateWindow(title);

            if (widthToMax || heightToMax)
            {
                var ioHelper = new WindowInteropHelper(window.Owner);
                var hWnd = ioHelper.Handle;
                var screen = Screen.FromHandle(hWnd);
                var dpi = VisualTreeHelper.GetDpi(window);

                if (heightToMax)
                {
                    window.MinHeight = height;
                    window.Height = screen.WorkingArea.Height * 0.96D / dpi.DpiScaleY;
                }
                else
                {
                    window.Height = height;
                }

                if (widthToMax)
                {
                    window.MinWidth = width;
                    window.Width = screen.WorkingArea.Width * 0.96D / dpi.DpiScaleY;
                }
                else
                {
                    window.Width = width;
                }
            }
            else
            {
                window.Width = width;
                window.Height = height;
            }

            PositionWindow(window);

            return window;
        }



        private static Window CreateWindow(string title, bool showMaximizeButton = false, bool showMinimizeButton = false)
        {
            var window = API.Instance.Dialogs.CreateWindow(new WindowCreationOptions { ShowCloseButton = true, ShowMaximizeButton = showMaximizeButton, ShowMinimizeButton = showMinimizeButton });
            window.Owner = API.Instance.Dialogs.GetCurrentAppWindow();
            window.Title = title;

            return window;
        }

        private static void PositionWindow(Window window) => window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }
}
