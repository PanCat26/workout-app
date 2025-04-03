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
        public WishListTab()
        {
            this.InitializeComponent();
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
                products.Add(productRepository.GetById(wishlistItem.ProductID));
            }

            ProductsGridView.ItemsSource = products;

        }

        private void BackToMainPageButton(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
