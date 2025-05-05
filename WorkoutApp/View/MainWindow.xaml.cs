// MainWindow.xaml.cs
namespace WorkoutApp.View
{
    using System;
    using Microsoft.UI;
    using Microsoft.UI.Windowing;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Graphics;
    using WinRT.Interop;

    public sealed partial class MainWindow : Window
    {
        public static Frame? AppFrame { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();
            AppFrame = this.MainFrame;
            this.SetFixedSize(1440, 720);
            MainPage mainPage = new MainPage();
            this.MainFrame.Navigate(typeof(MainPage));
        }




        private void SetFixedSize(int width, int height)
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow? appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                appWindow.Resize(new SizeInt32(width, height));
            }
        }
    }
}
