using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Windows;



namespace WorkoutApp.View.ProductTab
{
    public sealed partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            this.InitializeComponent();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string updatedProductName = ProductNameTextBox.Text;
            string updatedProductPrice = ProductPriceTextBox.Text;
            string updatedDescription = DescriptionTextBox.Text;
            string updatedSizes = SizesTextBox.Text;
            string updatedColors= ColorsTextBox.Text;
            string updatedQuantity= QuantityTextBox.Text;

            // You can add logic to save the updated product here

            
            this.Close();

            
        }

    }
}
