using System;
using Microsoft.UI.Xaml.Controls;


namespace WorkoutApp.View
{
    public sealed partial class SizeFilter : UserControl
    {
        public SizeFilter()
        {
            this.InitializeComponent();
        }

        public event EventHandler<string> SizeChanged;

        private void SizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedSize = (SizeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedSize))
            {
                this.SizeChanged?.Invoke(this, selectedSize);
            }
        }

        public void ResetFilter()
        {
            this.SizeComboBox.SelectedIndex = -1;
        }
    }
}
