using Microsoft.UI;
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
    public sealed partial class MainWindow : Window
    {
        private List<Product> allProducts = new List<Product>();
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
            allProducts = products;
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

        private void BuildDynamicFilters(string category)
        {
            FilterOptionsPanel.Children.Clear();
            FilterOptionsPanel.Visibility = Visibility.Visible;

            if (category == "clothing")
            {
                // Size Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Size", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox sizeCombo = new ComboBox();
                sizeCombo.Items.Add(new ComboBoxItem { Content = "S" });
                sizeCombo.Items.Add(new ComboBoxItem { Content = "M" });
                sizeCombo.Items.Add(new ComboBoxItem { Content = "L" });
                sizeCombo.Items.Add(new ComboBoxItem { Content = "XL" });
                sizeCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("clothing");
                FilterOptionsPanel.Children.Add(sizeCombo);

                // Color Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Color", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox colorCombo = new ComboBox();
                colorCombo.Items.Add(new ComboBoxItem { Content = "Black" });
                colorCombo.Items.Add(new ComboBoxItem { Content = "Red" });
                colorCombo.Items.Add(new ComboBoxItem { Content = "Blue" });
                colorCombo.Items.Add(new ComboBoxItem { Content = "White" });
                colorCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("clothing");
                FilterOptionsPanel.Children.Add(colorCombo);
            }
            else if (category == "creatine")
            {
                // Brand Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Brand", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox brandCombo = new ComboBox();
                brandCombo.Items.Add(new ComboBoxItem { Content = "Optimum" });
                brandCombo.Items.Add(new ComboBoxItem { Content = "MyProtein" });
                brandCombo.Items.Add(new ComboBoxItem { Content = "Bulk" });
                brandCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("creatine");

                brandCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("creatine");
                FilterOptionsPanel.Children.Add(brandCombo);

                // Weight Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Weight", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox weightCombo = new ComboBox();
                weightCombo.Items.Add(new ComboBoxItem { Content = "100g" });
                weightCombo.Items.Add(new ComboBoxItem { Content = "300g" });
                weightCombo.Items.Add(new ComboBoxItem { Content = "500g" });
                weightCombo.Items.Add(new ComboBoxItem { Content = "1kg" });
                weightCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("creatine");

                FilterOptionsPanel.Children.Add(weightCombo);

                // Form Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Form", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox formCombo = new ComboBox();
                formCombo.Items.Add(new ComboBoxItem { Content = "Powder" });
                formCombo.Items.Add(new ComboBoxItem { Content = "Capsules" });
                formCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("creatine");
                FilterOptionsPanel.Children.Add(formCombo);
            }
            else if (category == "protein")
            {
                // Flavor Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Flavor", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox flavorCombo = new ComboBox();
                flavorCombo.Items.Add(new ComboBoxItem { Content = "Vanilla" });
                flavorCombo.Items.Add(new ComboBoxItem { Content = "Chocolate" });
                flavorCombo.Items.Add(new ComboBoxItem { Content = "Strawberry" });
                flavorCombo.Items.Add(new ComboBoxItem { Content = "Unflavored" });
                flavorCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("protein");

                FilterOptionsPanel.Children.Add(flavorCombo);

                // Type Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Type", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox typeCombo = new ComboBox();
                typeCombo.Items.Add(new ComboBoxItem { Content = "Whey" });
                typeCombo.Items.Add(new ComboBoxItem { Content = "Isolate" });
                typeCombo.Items.Add(new ComboBoxItem { Content = "Casein" });
                typeCombo.Items.Add(new ComboBoxItem { Content = "Plant-Based" });
                typeCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("protein");

                FilterOptionsPanel.Children.Add(typeCombo);
            }
            else if (category == "accessories")
            {
                // Type Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Type", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox accTypeCombo = new ComboBox();
                accTypeCombo.Items.Add(new ComboBoxItem { Content = "Bag" });
                accTypeCombo.Items.Add(new ComboBoxItem { Content = "Bottle" });
                accTypeCombo.Items.Add(new ComboBoxItem { Content = "Strap" });
                accTypeCombo.Items.Add(new ComboBoxItem { Content = "Mat" });
                accTypeCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("accessories");

                FilterOptionsPanel.Children.Add(accTypeCombo);

                // Color Filter
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Color", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox accColorCombo = new ComboBox();
                accColorCombo.Items.Add(new ComboBoxItem { Content = "Black" });
                accColorCombo.Items.Add(new ComboBoxItem { Content = "Blue" });
                accColorCombo.Items.Add(new ComboBoxItem { Content = "Pink" });
                accColorCombo.Items.Add(new ComboBoxItem { Content = "Camo" });
                accColorCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("accessories");

                FilterOptionsPanel.Children.Add(accColorCombo);
            }
            else
            {
                // Default or All Products
                FilterOptionsPanel.Children.Add(new TextBlock { Text = "Sort By", Margin = new Thickness(0, 10, 0, 2), Foreground = new SolidColorBrush(Colors.White) });
                ComboBox sortCombo = new ComboBox();
                sortCombo.Items.Add(new ComboBoxItem { Content = "Price: Low to High" });
                sortCombo.Items.Add(new ComboBoxItem { Content = "Price: High to Low" });
                sortCombo.Items.Add(new ComboBoxItem { Content = "Name A-Z" });
                sortCombo.SelectionChanged += (s, e) => OnFilterOptionChanged("accessories");

                FilterOptionsPanel.Children.Add(sortCombo);
            }
        }

        private void OnFilterOptionChanged(string category)
        {
            // Create a dictionary to collect selected values
            Dictionary<string, string> selectedFilters = new Dictionary<string, string>();

            foreach (var child in FilterOptionsPanel.Children)
            {
                if (child is ComboBox comboBox &&
                    comboBox.SelectedItem is ComboBoxItem selectedItem &&
                    comboBox.Tag is string tag)
                {
                    selectedFilters[tag] = selectedItem.Content.ToString();
                }
            }

            // Filter logic depending on category
            var filtered = allProducts.AsEnumerable();

            switch (category)
            {
                case "clothing":
                    filtered = filtered.Where(p =>
                        (p.Name.ToLower().Contains("bra") || p.Name.ToLower().Contains("leggings") || p.Name.ToLower().Contains("shorts")) &&
                        (selectedFilters.ContainsKey("Size") ? p.Name.ToLower().Contains(selectedFilters["Size"].ToLower()) : true) &&
                        (selectedFilters.ContainsKey("Color") ? p.Name.ToLower().Contains(selectedFilters["Color"].ToLower()) : true));
                    break;

                case "creatine":
                    filtered = filtered.Where(p =>
                        p.Name.ToLower().Contains("creatine") &&
                        (selectedFilters.ContainsKey("Brand") ? p.Name.ToLower().Contains(selectedFilters["Brand"].ToLower()) : true) &&
                        (selectedFilters.ContainsKey("Weight") ? p.Name.ToLower().Contains(selectedFilters["Weight"].ToLower()) : true) &&
                        (selectedFilters.ContainsKey("Form") ? p.Name.ToLower().Contains(selectedFilters["Form"].ToLower()) : true));
                    break;

                case "protein":
                    filtered = filtered.Where(p =>
                        p.Name.ToLower().Contains("protein") &&
                        (selectedFilters.ContainsKey("Flavor") ? p.Name.ToLower().Contains(selectedFilters["Flavor"].ToLower()) : true) &&
                        (selectedFilters.ContainsKey("Type") ? p.Name.ToLower().Contains(selectedFilters["Type"].ToLower()) : true));
                    break;

                case "accessories":
                    filtered = filtered.Where(p =>
                        (p.Name.ToLower().Contains("bag") || p.Name.ToLower().Contains("bottle") || p.Name.ToLower().Contains("strap") || p.Name.ToLower().Contains("mat")) &&
                        (selectedFilters.ContainsKey("Type") ? p.Name.ToLower().Contains(selectedFilters["Type"].ToLower()) : true) &&
                        (selectedFilters.ContainsKey("Color") ? p.Name.ToLower().Contains(selectedFilters["Color"].ToLower()) : true));
                    break;

                default:
                    // Apply generic filtering or sorting if needed
                    break;
            }

            ProductsGridView.ItemsSource = filtered.ToList();
        }

        private void OnFilterAllProducts(object sender, RoutedEventArgs e)
        {
            BuildDynamicFilters("product");

            var filtered = allProducts
                .Where(p => p.Name.ToLower().Contains("bra") || p.Name.ToLower().Contains("leggings") || p.Name.ToLower().Contains("shorts"))
                .ToList();

            ProductsGridView.ItemsSource = filtered;
        }
        private void OnFilterCreatine(object sender, RoutedEventArgs e)
        {
            BuildDynamicFilters("creatine");

            var filtered = allProducts
                .Where(p => p.Name.ToLower().Contains("bra") || p.Name.ToLower().Contains("leggings") || p.Name.ToLower().Contains("shorts"))
                .ToList();

            ProductsGridView.ItemsSource = filtered;
        }
        private void OnFilterProtein(object sender, RoutedEventArgs e)
        {
            BuildDynamicFilters("protein");

            var filtered = allProducts
                .Where(p => p.Name.ToLower().Contains("bra") || p.Name.ToLower().Contains("leggings") || p.Name.ToLower().Contains("shorts"))
                .ToList();

            ProductsGridView.ItemsSource = filtered;
        }
        private void OnFilterVitamins(object sender, RoutedEventArgs e)
        {
            BuildDynamicFilters("vitamins");

            var filtered = allProducts
                .Where(p => p.Name.ToLower().Contains("bra") || p.Name.ToLower().Contains("leggings") || p.Name.ToLower().Contains("shorts"))
                .ToList();

            ProductsGridView.ItemsSource = filtered;
        }
        private void OnFilterClothing(object sender, RoutedEventArgs e)
        {
            BuildDynamicFilters("clothing");

            var filtered = allProducts
                .Where(p => p.Name.ToLower().Contains("bra") || p.Name.ToLower().Contains("leggings") || p.Name.ToLower().Contains("shorts"))
                .ToList();

            ProductsGridView.ItemsSource = filtered;
        }
        private void OnFilterAccessories(object sender, RoutedEventArgs e)
        {
            BuildDynamicFilters("accessories");

            var filtered = allProducts
                .Where(p => p.Name.ToLower().Contains("bra") || p.Name.ToLower().Contains("leggings") || p.Name.ToLower().Contains("shorts"))
                .ToList();

            ProductsGridView.ItemsSource = filtered;
        }
    }
}
