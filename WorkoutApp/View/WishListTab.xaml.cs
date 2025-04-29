using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using WorkoutApp.Components;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using WorkoutApp.Data.Database;

namespace WorkoutApp.View
{
    public sealed partial class WishListTab : UserControl
    {
        private Window parent { get; set; }

        public WishListTab()
        {
            this.InitializeComponent();
            LoadProducts();
        }

        public WishListTab(Window parent)
        {
            this.InitializeComponent();
            this.parent = parent;
            LoadProducts();
        }

        private async void LoadProducts()
        {
            // Setup the dbService first
            var connectionString = "Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            var dbConnectionFactory = new SqlDbConnectionFactory(connectionString);
            var dbService = new DbService(dbConnectionFactory);

            ProductRepository productRepository = new ProductRepository(dbService);

            // Uncomment and use the repository
            List<IProduct> products = new List<IProduct>();

            /*
            WishlistItemRepository wishlistItemRepository = new WishlistItemRepository(dbService);
            var wishListItems = await wishlistItemRepository.GetAllAsync();

            foreach (var wishlistItem in wishListItems)
            {
                var product = await productRepository.GetByIdAsync(wishlistItem.ProductID);
                if (product != null)
                    products.Add(product);
            }
            */

            ProductsGridView.ItemsSource = products;
        }

        private void BackToMainPageButton(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            parent.Close();
            main.Activate();
        }
    }
}
