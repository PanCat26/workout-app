using System;
using Microsoft.UI.Xaml.Controls;

namespace WorkoutApp.View.Components
{
    public sealed partial class ColorFilter : UserControl
    {
        public ColorFilter()
        {
            this.InitializeComponent();
        }

        public event EventHandler<string> ColorChanged;

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedColor = (ColorComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedColor))
            {
                this.ColorChanged?.Invoke(this, selectedColor);
            }
        }
    }
}
