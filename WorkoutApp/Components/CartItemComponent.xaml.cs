using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.Components
{
    public sealed partial class CartItemComponent : UserControl
    {
        private StackPanel parent { get; set; }
        /*private CartService cartService { get; set; }
        private CartItem cartItem { get; set; }
*/
        private Func<int> callBack { get; set; }

        public CartItemComponent()
        {
            this.InitializeComponent();
        }
        /*
                public CartItemComponent(CartItem cartItem, StackPanel parent, Func<int> callBack)
                {
                    this.InitializeComponent();

                    this.cartItem = cartItem;
                    this.cartService = new CartService(new CartItemRepository(), new ProductRepository());
                    this.parent = parent;
                    this.callBack = callBack;

                    setDataContext();
                }
        */
        private void setDataContext()
        {
            /*IProduct product = cartItem.GetProduct(new ProductRepository());

            this.DataContext = new CartItemViewModel(product.Name, product.FileUrl, product.Price, cartItem.Quantity);
            //this.DataContext = new CartItemViewModel("product.Name", "product.FileUrl", "product.Price.ToString()", cartItem.Quantity.ToString());*/
        }

        private void deacreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            /*cartService.DecreaseQuantity(cartItem);
            cartItem = cartService.GetCartItemById((int)cartItem.Id);

            if (cartItem.Quantity == 0)
            {
                cartService.RemoveCartItem(cartItem);
                parent.Children.Remove(this);
                callBack();
                return;
            }

            setDataContext();
            callBack();*/
        }

        private void increaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            /*cartService.IncreaseQuantity(cartItem);
            cartItem = cartService.GetCartItemById((int)cartItem.Id);
            setDataContext();
            callBack();*/
        }

        private void removeProductButton_Click(object sender, RoutedEventArgs e)
        {
            /*cartService.RemoveCartItem(cartItem);
            parent.Children.Remove(this);
            callBack();*/
        }
    }
}
