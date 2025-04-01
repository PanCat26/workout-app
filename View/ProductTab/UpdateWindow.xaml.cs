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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View.ProductTab
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
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
            string updatedColors = ColorsTextBox.Text;
            string updatedQuantity = QuantityTextBox.Text;

            // You can add logic to save the updated product here


            this.Close();


        }

    }
}
