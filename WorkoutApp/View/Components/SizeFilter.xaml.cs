// <copyright file="SizeFilter.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View.Components
{
    using System;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Represents a user control that provides a size filter functionality.
    /// </summary>
    public sealed partial class SizeFilter : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SizeFilter"/> class.
        /// </summary>
        public SizeFilter()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Occurs when the selected size is changed.
        /// </summary>
        public event EventHandler<string> SizeChanged;

        /// <summary>
        /// Resets the size filter to its default state.
        /// </summary>
        public void ResetFilter()
        {
            this.SizeComboBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Handles the selection change event of the size combo box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void SizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedSize = (this.SizeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedSize))
            {
                this.SizeChanged?.Invoke(this, selectedSize);
            }
        }
    }
}
