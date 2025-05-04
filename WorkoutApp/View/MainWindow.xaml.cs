// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View
{
    using System;
    using Microsoft.UI;
    using Microsoft.UI.Windowing;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Graphics;
    using WinRT.Interop;

    /// <summary>
    /// Represents the main window of the WorkoutApp application.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            AppFrame = this.MainFrame;
            this.SetFixedSize(1440, 720);
            MainPage mainPage = new MainPage();
            this.MainFrame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Gets the application's main navigation frame.
        /// </summary>
        public static Frame? AppFrame { get; private set; }

        /// <summary>
        /// Sets a fixed size for the application window.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
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
