using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
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
                new Product { Name = "Tricou Negru XL", Price = 29.99, Quantity = 2, Image = "./photos/tricou.jpg" },
                new Product { Name = "Gantera 10 KG", Price = 49.99, Quantity = 1, Image = "./photos/gantera.jpg" },
                new Product { Name = "Proteina Gym Beam", Price = 19.99, Quantity = 3, Image = "./photos/protein.jpg" },
            };

            ProductsGridView.ItemsSource = products;
        }

        private void deacreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void increaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void removeProductButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void proceedToCheckoutButton(object sender, RoutedEventArgs e)
        {
            Window window = new Payment();
            window.Activate();
            this.Close();
        }
    }
    public class PriceTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Product product)
            {
                return "$" + (product.Price * product.Quantity).ToString();
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }

        public string Type { get; set; }

        public int Quantity { get; set; }

        public List<string>? Colors { get; set; }
        public List<string>? Sizes { get; set; }
        public List<string>? Weights { get; set; }
    }
}
