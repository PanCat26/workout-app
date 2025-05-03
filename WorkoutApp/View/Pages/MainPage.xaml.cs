// <copyright file="MainPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using WorkoutApp.Models;
    using WorkoutApp.ViewModel;

    /// <summary>
    /// A page that displays the main list of products and allows category filtering.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MainPageViewModel mainPageViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.mainPageViewModel = new MainPageViewModel();
        }

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.LoadProducts();
        }

        private async Task LoadProducts()
        {
            IEnumerable<Product> products = await this.mainPageViewModel.GetAllProductsAsync();
            this.ProductListViewControl.SetProducts(products);
        }

        private void VerticalProductListControl_ProductClicked(object sender, int productID)
        {
            MainWindow.AppFrame.Navigate(typeof(ProductDetailPage), productID);
        }

        private async void CategorySelector_SelectionChanged(object sender, int selectedCategoryID)
        {
            this.mainPageViewModel.SetSelectedCategoryID(selectedCategoryID);
            await this.LoadProducts();
        }

        private void ColorSelector_SelectionChanged(object sender, string color)
        {
            //
        }

        private void BuildCategoryFilters(int categoryId)
        {
            /*
            while (FilterOptionsPanel.Children.Count > 2)
            {
                FilterOptionsPanel.Children.RemoveAt(2);
            }

            switch (categoryId)
            {
                case 1:
                    AddFilterCombo("Size", new List<string> { "S", "M", "L", "XL" }, categoryId);
                    AddFilterCombo("Color", new List<string> { "Black", "Red", "Blue", "White" }, categoryId);
                    break;

                case 2:
                    AddFilterCombo("Size", new List<string> { "20g", "100g", "250g", "500g", "1kg" }, categoryId);
                    break;

                case 3:
                    // Future use.
                    break;
            }

            var clearButton = new Button
            {
                Content = "Clear Filters",
                Margin = new Thickness(0, 20, 0, 0),
                Width = 140,
                HorizontalAlignment = HorizontalAlignment.Left,
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
            /*
            FilterOptionsPanel.Children.Add(new TextBlock
            {
                Text = tag,
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 10, 0, 2),
            });

            var combo = new ComboBox { Tag = tag, Width = 140 };

            foreach (string option in options)
            {
                combo.Items.Add(new ComboBoxItem { Content = option });
            }

            combo.SelectionChanged += (s, e) => this.ApplyCategoryFilter(categoryId);
            FilterOptionsPanel.Children.Add(combo);*/
        }

        private void ApplyCategoryFilter(int categoryId)
        {
            /*
            Dictionary<string, string> selectedFilters = new();

            foreach (var child in FilterOptionsPanel.Children)
            {
                if (child is ComboBox comboBox &&
                    comboBox.SelectedItem is ComboBoxItem selectedItem &&
                    comboBox.Tag is string tag)
                {
                    selectedFilters[tag] = selectedItem.Content.ToString();
                }
            }

            IEnumerable<IProduct> filtered = allProducts.Where(p => p.CategoryID == categoryId);

            switch (categoryId)
            {
                case 1:
                    filtered = filtered.OfType<ClothesProduct>().Where(p =>
                        (!selectedFilters.ContainsKey("Size") || p.Size.Contains(selectedFilters["Size"], StringComparison.OrdinalIgnoreCase)) &&
                        (!selectedFilters.ContainsKey("Color") || p.Attributes.Contains(selectedFilters["Color"], StringComparison.OrdinalIgnoreCase)));
                    break;

                case 2:
                    filtered = filtered.OfType<FoodProduct>().Where(p =>
                        (!selectedFilters.ContainsKey("Size") || p.Size.Contains(selectedFilters["Size"], StringComparison.OrdinalIgnoreCase)));
                    break;

                case 3:
                    break;
            }

            ProductsGridView.ItemsSource = filtered.ToList();
            */
        }
    }
}
