using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WorkoutApp.View;

namespace WorkoutApp
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
           
            var products = new List<Product>
            {
                new Product { Name = "Bra", Price = "$29.99", Image = "bra_image.jpg" },
                new Product { Name = "Leggings", Price = "$49.99", Image = "leggings_image.jpg" },
                new Product { Name = "Shorts", Price = "$19.99", Image = "shorts_image.jpg" },
                new Product { Name = "Bra", Price = "$29.99", Image = "bra_image.jpg" },
                new Product { Name = "Leggings", Price = "$49.99", Image = "leggings_image.jpg" },
                new Product { Name = "Shorts", Price = "$19.99", Image = "shorts_image.jpg" },
                new Product { Name = "Bra", Price = "$29.99", Image = "bra_image.jpg" },
                new Product { Name = "Leggings", Price = "$49.99", Image = "leggings_image.jpg" },
                new Product { Name = "Shorts", Price = "$19.99", Image = "shorts_image.jpg" },
                new Product { Name = "Bra", Price = "$29.99", Image = "bra_image.jpg" },
                new Product { Name = "Leggings", Price = "$49.99", Image = "leggings_image.jpg" },
                new Product { Name = "Shorts", Price = "$19.99", Image = "shorts_image.jpg" },

            };
            ProductsGridView.ItemsSource = products;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow addProductWindow = new AddProductWindow();

            
            addProductWindow.ProductAdded += (newProduct) =>
            {
                if (newProduct != null)
                {
                    var products = ProductsGridView.ItemsSource as List<Product>;
                    if (products != null)
                    {
                        products.Add(newProduct);
                        ProductsGridView.ItemsSource = null;
                        ProductsGridView.ItemsSource = products;
                    }
                }
            };

            addProductWindow.Activate();
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }

        public string Type { get; set; }

        public string Quantity { get; set; }

        public List<string>? Colors { get; set; }
        public List<string>? Sizes { get; set; }
        public List<string>? Weights { get; set; }
    }
}
