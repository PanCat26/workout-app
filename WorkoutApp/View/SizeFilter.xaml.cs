using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;


namespace WorkoutApp.View
{
    public sealed partial class SizeFilter : UserControl
    {
        public SizeFilter()
        {
            this.InitializeComponent();
        }

        private void SizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSize = (SizeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            System.Diagnostics.Debug.WriteLine($"[SizeFilter] Selected size: {selectedSize}");
        }
    }
}
