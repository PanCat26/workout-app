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
using WorkoutApp.Repository;
using WorkoutApp.Service;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShoppingCart : Window
    {
        public ShoppingCart()
        {
            this.InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            CartItemRepository cartItemRepository = new CartItemRepository();
            ProductRepository productRepository = new ProductRepository();
            CartService cartService = new CartService(cartItemRepository, productRepository);

            var cartItems = cartService.GetCartItems();
            foreach (var cartItem in cartItems)
            {
                ProductsStackPanel.Children.Add(new CartItemComponent(cartItem, ProductsStackPanel));
            }
        }

        private void proceedToCheckoutButton(object sender, RoutedEventArgs e)
        {
            Window window = new Payment();
            window.Activate();
            this.Close();
        }


    }
}
