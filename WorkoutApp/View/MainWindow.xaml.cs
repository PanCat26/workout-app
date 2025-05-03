// MainWindow.xaml.cs
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;

namespace WorkoutApp.View
{
    public sealed partial class MainWindow : Window
    {
        //private List<IProduct> allProducts = new List<IProduct>();

        public static Frame? AppFrame { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();
            AppFrame = this.MainFrame;
            LoadProducts();
        }

        private void LoadProducts()
        {
            /*
            ProductRepository productRepository = new ProductRepository();
            allProducts = productRepository.GetProducts();
            ProductsGridView.ItemsSource = allProducts;
            */
        }

        //private void LoadProducts()
        //{
        //    allProducts = new List<IProduct>
        //    {
        //        new ClothesProduct(1, "Bra", 29.99, 10, 1, "Black", "S", "Comfortable sports bra", "bra_image.jpg", true),
        //        new ClothesProduct(2, "Leggings", 49.99, 15, 1, "Red", "M", "High-waisted leggings", "leggings_image.jpg", true),
        //        new ClothesProduct(3, "Shorts", 19.99, 20, 1, "Blue", "L", "Breathable gym shorts", "shorts_image.jpg", true),
        //        new ClothesProduct(4, "Tank Top", 24.99, 12, 1, "White", "M", "Lightweight tank top", "tanktop_image.jpg", true),
        //        new ClothesProduct(5, "Jacket", 89.99, 5, 1, "Black", "L", "Warm gym jacket", "jacket_image.jpg", true),
        //        new ClothesProduct(6, "Sports Tights", 39.99, 18, 1, "Blue", "S", "Flexible sports tights", "tight_image.jpg", true),
        //        new ClothesProduct(7, "Hoodie", 59.99, 8, 1, "Red", "XL", "Cozy training hoodie", "hoodie_image.jpg", true),
        //        new ClothesProduct(8, "Shorts", 21.99, 25, 1, "Black", "M", "Mesh training shorts", "shorts2_image.jpg", true),

        //        // Food
        //        new FoodProduct(9, "Whey Protein", 49.99, 40, 2, "1kg", "Vanilla flavored whey protein", "whey_image.jpg", true),
        //        new FoodProduct(10, "Protein Bar", 2.99, 100, 2, "20g", "Chocolate protein snack", "bar_image.jpg", true),
        //        new FoodProduct(11, "Creatine Powder", 19.99, 30, 2, "300g", "Micronized creatine powder", "creatine_image.jpg", true),
        //        new FoodProduct(12, "BCAA", 34.99, 22, 2, "500g", "Amino acids for recovery", "bcaa_image.jpg", true),

        //        // Accessories
        //        new AccessoryProduct(13, "Gym Bag", 39.99, 12, 3, "Spacious gym bag", "bag_image.jpg", true),
        //        new AccessoryProduct(14, "Wrist Straps", 14.99, 50, 3, "Lifting support straps", "straps_image.jpg", true),
        //        new AccessoryProduct(15, "Water Bottle", 9.99, 80, 3, "Shatterproof bottle", "bottle_image.jpg", true)


        //    };
        //    ProductsGridView.ItemsSource = allProducts;
        //}

        private void OnFilterClick(object sender, RoutedEventArgs e)
        {
            FilterOptionsPanel.Children.Clear();
            FilterOptionsPanel.Visibility = Visibility.Visible;

            var categorySelector = new ComboBox { Width = 160, Margin = new Thickness(0, 0, 0, 10) };
            categorySelector.Items.Add(new ComboBoxItem { Content = "Clothes", Tag = 1 });
            categorySelector.Items.Add(new ComboBoxItem { Content = "Food", Tag = 2 });
            categorySelector.Items.Add(new ComboBoxItem { Content = "Accessories", Tag = 3 });
            categorySelector.SelectionChanged += CategorySelector_SelectionChanged;

            FilterOptionsPanel.Children.Add(new TextBlock
            {
                Text = "Category",
                Foreground = new SolidColorBrush(Colors.White)
            });
            FilterOptionsPanel.Children.Add(categorySelector);
        }

        private void CategorySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selected && selected.Tag is int categoryId)
            {
                ProductsGridView.ItemsSource = allProducts.Where(p => p.CategoryID == categoryId).ToList();
                BuildCategoryFilters(categoryId);
            }
            */
        }

        private void BuildCategoryFilters(int categoryId)
        {
            /*
            while (FilterOptionsPanel.Children.Count > 2)
                FilterOptionsPanel.Children.RemoveAt(2);

            switch (categoryId)
            {
                case 1: // Clothes
                    AddFilterCombo("Size", new List<string> { "S", "M", "L", "XL" }, categoryId);
                    AddFilterCombo("Color", new List<string> { "Black", "Red", "Blue", "White" }, categoryId);
                    break;

                case 2: // Food
                    AddFilterCombo("Size", new List<string> { "20g", "100g", "250g", "500g", "1kg" }, categoryId);
                    break;

                case 3: // Accessories
                    //future
                    break;
            }

            var clearButton = new Button
            {
                Content = "Clear Filters",
                Margin = new Thickness(0, 20, 0, 0),
                Width = 140,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            clearButton.Click += (s, e) =>
            {
                ProductsGridView.ItemsSource = allProducts;
                FilterOptionsPanel.Visibility = Visibility.Collapsed;
            };
            FilterOptionsPanel.Children.Add(clearButton);
            */
        }

        private void AddFilterCombo(string tag, List<string> options, int categoryId)
        {
            FilterOptionsPanel.Children.Add(new TextBlock
            {
                Text = tag,
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 10, 0, 2)
            });

            var combo = new ComboBox { Tag = tag, Width = 140 };
            foreach (var option in options)
                combo.Items.Add(new ComboBoxItem { Content = option });

            combo.SelectionChanged += (s, e) => ApplyCategoryFilter(categoryId);
            FilterOptionsPanel.Children.Add(combo);
        }

        private void ApplyCategoryFilter(int categoryId)
        {
            /*
            Dictionary<string, string> selectedFilters = new();

            foreach (var child in FilterOptionsPanel.Children)
            {
                if (child is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem && comboBox.Tag is string tag)
                {
                    selectedFilters[tag] = selectedItem.Content.ToString();
                }
            }

            IEnumerable<IProduct> filtered = allProducts.Where(p => p.CategoryID == categoryId);

            switch (categoryId)
            {
                case 1: // Clothes
                    filtered = filtered.OfType<ClothesProduct>().Where(p =>
                        (!selectedFilters.ContainsKey("Size") || p.Size.Contains(selectedFilters["Size"], StringComparison.OrdinalIgnoreCase)) &&
                        (!selectedFilters.ContainsKey("Color") || p.Attributes.Contains(selectedFilters["Color"], StringComparison.OrdinalIgnoreCase)));
                    break;

                case 2: // Food
                    filtered = filtered.OfType<FoodProduct>().Where(p =>
                        (!selectedFilters.ContainsKey("Size") || p.Size.Contains(selectedFilters["Size"], StringComparison.OrdinalIgnoreCase)));
                    break;

                case 3: // Accessories
                    break;
            }

            ProductsGridView.ItemsSource = filtered.ToList();
            */
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow addProductWindow = new AddProductWindow();
            addProductWindow.Activate();
            this.Close();
        }

        private void CartMenu_Click(object sender, RoutedEventArgs e)
        {
            Window cart = new ShoppingCart();
            cart.Activate();
            this.Close();
        }
        private void WishList_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new WishListTab(this);
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            /*
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                string searchTerm = SearchBox.Text.Trim();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    ProductsGridView.ItemsSource = allProducts;
                }
                else
                {
                    //ProductsGridView.ItemsSource = allProducts;
                    IEnumerable<IProduct> filtered = allProducts.Where(p => p.Name.Contains(searchTerm));
                    ProductsGridView.ItemsSource = filtered;
                }
            }*/
        }

        private void SeeProduct_Click(object sender, RoutedEventArgs e)
        {   /*
            var button = sender as TextBlock;
            string ProductName = button.Text;
            this.Content = new ProductTab.ProductTab(allProducts.Where(p => p.Name.Equals(ProductName)).First(), this);
            *///if (button?.Tag is Product selectedProduct)
            //{
            //    this.Content = new ProductTab.ProductTab(allProducts[1]);
            //}
        }
    }
}
