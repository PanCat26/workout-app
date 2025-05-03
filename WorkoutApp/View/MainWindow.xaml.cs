// MainWindow.xaml.cs
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Graphics;
using WinRT.Interop;

namespace WorkoutApp.View
{
    public sealed partial class MainWindow : Window
    {
        public static Frame? AppFrame { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();
            AppFrame = this.MainFrame;
            this.SetFixedSize(1440, 720);
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

        private void SetFixedSize(int width, int height)
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow? appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                appWindow.Resize(new SizeInt32(width, height));
            }
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

        /*
        /// <summary>
        /// Handles the Click event for the "View Product 1" button.
        /// Opens the ProductDetailPage in a new window for product ID 2.
        /// </summary>
        private void ViewProduct1_Click(object sender, RoutedEventArgs e)
        {
            // Define the product ID to navigate to
            int productIdToNavigate = 2;

            // Initialize dependencies for the ProductService.
            // In a real application, you would typically use a Dependency Injection container here.
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            // Replace with your actual connection string or get from config
            var connectionFactory = new DbConnectionFactory(connectionString);
            var dbService = new DbService(connectionFactory);
            var productRepository = new ProductRepository(dbService);
            var productService = new ProductService(productRepository);

            // Create a new Window first
            //var newWindow = new Window();

            // Create a new instance of the ProductDetailPage, passing the new window to its constructor
            // This allows the ProductDetailPage to know its hosting window for internal navigation.
            var productDetailPage = new ProductDetailPage();

            // If ProductDetailPage ViewModel has a LoadProductAsync(int id) method:
            // This is the preferred approach for passing data after page creation.
            // You'll need to ensure your ProductDetailPage.xaml.cs has a public ViewModel property.
            // Use _ = ... to avoid awaiting in a void method.
            _ = productDetailPage.ViewModel.LoadProductAsync(productIdToNavigate);


            // Set the content of the new window to the ProductDetailPage
            //newWindow.Content = productDetailPage;

            // Set a title for the new window (optional)
            //newWindow.Title = $"Product Details (ID: {productIdToNavigate})";

            // Activate and show the new window
            //newWindow.Activate();
        }
        */
    }
}
