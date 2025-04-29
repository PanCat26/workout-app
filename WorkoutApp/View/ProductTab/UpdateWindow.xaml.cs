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
using WorkoutApp.Repository;
using WorkoutApp.Service;
using WorkoutApp.Models;
using WorkoutApp.Data.Database;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View.ProductTab
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateWindow : Window
    {

        private ProductService productService;
        private IProduct product;
        public UpdateWindow(IProduct product)
        {
            this.InitializeComponent();

            // Create the connection + dbService manually
            var connectionString = "Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            var dbConnectionFactory = new SqlDbConnectionFactory(connectionString);
            var dbService = new DbService(dbConnectionFactory);

            ProductRepository productRepository = new ProductRepository(dbService);
            this.productService = new ProductService(productRepository);
            this.product = product;

            ProductNameTextBox.Text = product.Name;
            ProductPriceTextBox.Text = product.Price.ToString();
            DescriptionTextBox.Text = product.Description;

            if (product.CategoryID == 1)
            {
                SizesTextBox.Text = ((ClothesProduct)product).Size;
                ColorsTextBox.Text = ((ClothesProduct)product).Attributes;
            }
            if (product.CategoryID == 2)
            {
                SizesTextBox.Text = ((FoodProduct)product).Size;
                ColorsTextBlock.Visibility = Visibility.Collapsed;
                ColorsTextBox.Visibility = Visibility.Collapsed;
            }
            if (product.CategoryID == 3)
            {
                ColorsTextBlock.Visibility = Visibility.Collapsed;
                ColorsTextBox.Visibility = Visibility.Collapsed;
                SizesTextBlock.Visibility = Visibility.Collapsed;
                SizesTextBox.Visibility = Visibility.Collapsed;
            }
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string updatedProductName = ProductNameTextBox.Text;
            string updatedProductPrice = ProductPriceTextBox.Text;
            string updatedDescription = DescriptionTextBox.Text;
            string updatedSizes = SizesTextBox.Text;
            string updatedColors = ColorsTextBox.Text;
            try
            {
                if (!float.TryParse(updatedProductPrice, out float price))
                {
                    throw new Exception("The price must be a numerical value!");
                }



                await this.productService.UpdateProductAsync(product.ID, updatedProductName, price, 15, product.CategoryID, updatedDescription, product.FileUrl, updatedColors, updatedSizes);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }


            MainWindow main = new MainWindow();
            this.Close();
            main.Activate();


        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Activate();
        }

    }
}
