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
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using WorkoutApp.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.Components
{
    public sealed partial class CartItemComponent : UserControl
    {
        private StackPanel parent { get; set; }
        private CartService cartService { get; set; }
        private CartItem cartItem { get; set; }

        private Func<int> callBack {  get; set; }

        public CartItemComponent(CartService cartService)
        {
            this.InitializeComponent();
            this.cartService = cartService;
        }

        public CartItemComponent(CartItem cartItem, StackPanel parent, Func<int> callBack)
        {
            this.InitializeComponent();

            this.cartItem = cartItem;
            this.parent = parent;
            this.callBack = callBack;

            setDataContext();
        }

        private void setDataContext()
        {
            IProduct product = cartItem.GetProduct(new ProductRepository());

            this.DataContext = new CartItemViewModel(product.Name, product.FileUrl, product.Price, cartItem.Quantity);
            //this.DataContext = new CartItemViewModel("product.Name", "product.FileUrl", "product.Price.ToString()", cartItem.Quantity.ToString());
        }

        private async void deacreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            await cartService.DecreaseQuantityAsync(cartItem);
            cartItem = await cartService.GetCartItemByIdAsync((int)cartItem.Id);
            
            if(cartItem.Quantity == 0)
            {
                await cartService.RemoveCartItemAsync(cartItem);
                parent.Children.Remove(this);
                callBack();
                return;
            }
            
            setDataContext();
            callBack();
        }

        private async void increaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            await cartService.IncreaseQuantityAsync(cartItem);
            cartItem = await cartService.GetCartItemByIdAsync((int)cartItem.Id);
            setDataContext();
            callBack();
        }

        private async void removeProductButton_Click(object sender, RoutedEventArgs e)
        {
            await cartService.RemoveCartItemAsync(cartItem);
            parent.Children.Remove(this);
            callBack();
        }
    }
}
