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
using WorkoutApp.Components;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View
{
    public sealed partial class WishListTab : UserControl
    {
        private Window parent {  get; set; }
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

        private void LoadProducts()
        {
            WishlistItemRepository wishlistItemRepository = new WishlistItemRepository();
            ProductRepository productRepository = new ProductRepository();
            productRepository.LoadData();

            var WishListItems = wishlistItemRepository.GetAll();

            List<IProduct> products = new List<IProduct>();
            foreach( var wishlistItem in WishListItems)
            {
                var product = productRepository.GetById(wishlistItem.ProductID);
                if(product != null)
                    products.Add(productRepository.GetById(wishlistItem.ProductID));
            }

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
