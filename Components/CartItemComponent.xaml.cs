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
using WorkoutApp.ViewModel;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.Components
{
    public sealed partial class CartItemComponent : UserControl
    {
        private StackPanel parent { get; set; }
        private CartService cartService { get; set; }
        private CartItem cartItem { get; set; }

        public CartItemComponent()
        {
            this.InitializeComponent();
        }

        public CartItemComponent(CartItem cartItem, StackPanel parent)
        {
            this.InitializeComponent();

            this.cartItem = cartItem; 
            this.cartService = new CartService(new CartItemRepository(), new ProductRepository());
            this.parent = parent;

            setDataContext();  
        }

        private void setDataContext()
        {
            Product product = cartItem.GetProduct(new ProductRepository());
            this.DataContext = new CartItemViewModel(product.Name, product.Image, product.Price, (int)cartItem.Quantity);
        }

        private void deacreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            cartService.DecreaseQuantity(cartItem);
            cartItem = cartService.GetCartItemById((int) cartItem.Id);
            setDataContext();
        }

        private void increaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            cartService.IncreaseQuantity(cartItem);
            cartItem = cartService.GetCartItemById((int) cartItem.Id);
            setDataContext();
        }

        private void removeProductButton_Click(object sender, RoutedEventArgs e)
        {
            cartService.RemoveCartItem(cartItem);
            parent.Children.Remove(this);
        }
    }
}
