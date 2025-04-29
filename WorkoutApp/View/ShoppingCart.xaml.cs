using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WorkoutApp.Components;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using WorkoutApp.Data.Database;

namespace WorkoutApp.View
{
    public sealed partial class ShoppingCart : Window
    {
        private double TotalAmount { get; set; }
        private readonly CartService cartService;
        private readonly ProductRepository productRepository;

        public ShoppingCart()
        {
            this.InitializeComponent();

            // Setup the database service
            var connectionString = "Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            var dbConnectionFactory = new SqlDbConnectionFactory(connectionString);
            var dbService = new DbService(dbConnectionFactory);

            // Create the repositories with dbService
            this.productRepository = new ProductRepository(dbService);
            var cartItemRepository = new CartItemRepository();

            // Create services
            this.cartService = new CartService(cartItemRepository, this.productRepository);

            LoadProducts();
            computeCost();
        }

        private void LoadProducts()
        {
            var cartItems = cartService.GetCartItems();
            foreach (var cartItem in cartItems)
            {
                ProductsStackPanel.Children.Add(new CartItemComponent(cartItem, ProductsStackPanel, computeCost));
            }
        }

        private int computeCost()
        {
            var cartItems = cartService.GetCartItems();

            double cost = 0;
            foreach (var cartItem in cartItems)
            {
                var product = cartItem.GetProductAsync(this.productRepository).GetAwaiter().GetResult();
                cost += cartItem.Quantity * product.Price;
            }

            TotalAmountTextBlock.Text = "Total amount: $" + string.Format("{0:0.##}", cost);
            if (cost < 100)
            {
                TotalCostTextBlock.Text = "Total cost: $" + string.Format("{0:0.##}", cost + 20) + " ($20 transport fee)";
                TotalAmount = cost + 20;
            }
            else
            {
                TotalCostTextBlock.Text = "Total cost: $" + string.Format("{0:0.##}", cost) + " (free transport)";
                TotalAmount = cost;
            }

            return 0;
        }

        private void proceedToCheckoutButton(object sender, RoutedEventArgs e)
        {
            Window window = new Payment(TotalAmount);
            window.Activate();
            this.Close();
        }

        private void BackButton(object sender, RoutedEventArgs e)
        {
            Window window = new MainWindow();
            window.Activate();
            this.Close();
        }
    }
}
