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
using WorkoutApp.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddProductWindow : Window
    {
        public AddProductWindow()
        {
            this.InitializeComponent();
        }

        public event Action<Product> ProductAdded;
        Product newProduct { get; set; }

        private void ProductTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedType = (ProductTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            ClothesPanel.Visibility = Visibility.Collapsed;
            FoodPanel.Visibility = Visibility.Collapsed;

            if (selectedType == "Clothes")
            {
                ClothesPanel.Visibility = Visibility.Visible;
            }
            else if (selectedType == "Food")
            {
                FoodPanel.Visibility = Visibility.Visible;
            }
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string selectedType = (ProductTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            newProduct = new Product
            {
                Name = NameTextBox.Text,
                Price = PriceTextBox.Text,
                Image = ImageTextBox.Text,
                Type = selectedType
            };

            if (selectedType == "Clothes")
            {
                newProduct.Colors = ColorsTextBox.Text.Split(',').Select(c => c.Trim()).ToList();
                newProduct.Sizes = SizesTextBox.Text.Split(',').Select(s => s.Trim()).ToList();
            }
            else if (selectedType == "Food")
            {
                newProduct.Weights = WeightsTextBox.Text.Split(',').Select(w => w.Trim()).ToList();
            }

            ProductAdded?.Invoke(newProduct);
            this.Close();
        }
    }
}
