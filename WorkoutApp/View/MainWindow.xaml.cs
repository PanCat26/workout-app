// MainWindow.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WorkoutApp.View
{
    public sealed partial class MainWindow : Window
    {
        public static Frame? AppFrame { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();
            AppFrame = this.MainFrame;
            MainPage mainPage = new MainPage();
            this.MainFrame.Navigate(typeof(MainPage));
        }

        private void OnFilterClick(object sender, RoutedEventArgs e)
        {
            /* FilterOptionsPanel.Children.Clear();
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
               FilterOptionsPanel.Children.Add(categorySelector);*/
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
