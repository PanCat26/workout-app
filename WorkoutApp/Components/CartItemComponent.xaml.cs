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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WorkoutApp.Data.Database;
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

        private DbService dbService { get; set; }

        public CartItemComponent()
        {
            this.InitializeComponent();
        }

        public CartItemComponent(CartItem cartItem, StackPanel parent, Func<int> callBack)
        {
            this.InitializeComponent();
            this.cartItem = cartItem;
            this.parent = parent;
            this.callBack = callBack;

            var connectionString = "Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"; // or use from config if you want
            var dbConnectionFactory = new SqlDbConnectionFactory(connectionString);
            var dbService = new DbService(dbConnectionFactory);

            this.cartService = new CartService(new CartItemRepository(), new ProductRepository(dbService));

            setDataContext().GetAwaiter().GetResult();
        }


        private async Task setDataContext()
        {
            IProduct product = await cartItem.GetProductAsync(new ProductRepository(dbService));

            this.DataContext = new CartItemViewModel(product.Name, product.FileUrl, product.Price, cartItem.Quantity);
        }

        private void deacreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            cartService.DecreaseQuantity(cartItem);
            cartItem = cartService.GetCartItemById((int)cartItem.Id);
            
            if(cartItem.Quantity == 0)
            {
                cartService.RemoveCartItem(cartItem);
                parent.Children.Remove(this);
                callBack();
                return;
            }
            
            setDataContext();
            callBack();
        }

        private void increaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            cartService.IncreaseQuantity(cartItem);
            cartItem = cartService.GetCartItemById((int)cartItem.Id);
            setDataContext();
            callBack();
        }

        private void removeProductButton_Click(object sender, RoutedEventArgs e)
        {
            cartService.RemoveCartItem(cartItem);
            parent.Children.Remove(this);
            callBack();
        }
    }
}
