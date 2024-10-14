using Playnite.SDK;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace LaunchMate.Utilities
{
    public static class WindowHelper
    {
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
