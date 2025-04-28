using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using WorkoutApp.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View.ProductTab
{
    public sealed partial class ProductTab : UserControl
    {
        public ProductDetailsViewModel ViewModel { get; set; }
        private readonly ProductService productService;
        private readonly WishlistService wishlistService;
        private readonly CartService cartService;
        private IProduct product;
        private Window parent { get; set; }
        public ProductTab(IProduct product, Window parent)
        {/*
            this.InitializeComponent();
            CartItemRepository cartItemRepository = new CartItemRepository();
            ProductRepository productRepository = new ProductRepository();
            WishlistItemRepository wishlistRepository = new WishlistItemRepository();
            this.cartService = new CartService(cartItemRepository, productRepository);
            this.wishlistService = new  WishlistService(wishlistRepository, productRepository);
            this.productService = new ProductService(productRepository);
            this.product = product;
            this.parent = parent;

            ViewModel = new ProductDetailsViewModel(productService, wishlistService, cartService, product);
            DataContext = ViewModel;
            PopulateColorFlyout();


            if(product.CategoryID == 2 || product.CategoryID == 3)
            {
                ColorSplitButton.Visibility = Visibility.Collapsed;
                ColorTextBlock.Visibility = Visibility.Collapsed;
            }
            if(product.CategoryID == 3)
            {
                SizeStackPanel.Visibility = Visibility.Collapsed;
            }*/
        }

        private void PopulateColorFlyout()
        {
            ColorFlyout.Items.Clear();

            foreach (var color in ViewModel.AvailableColors)
            {
                var menuItem = new MenuFlyoutItem
                {
                    Text = color,
                    Background = new SolidColorBrush(ConvertStringToColor(color))
                };

                menuItem.Click += (s, e) =>
                {
                    ViewModel.SelectedColor = color;
                };

                ColorFlyout.Items.Add(menuItem);
            }
        }

        private Windows.UI.Color ConvertStringToColor(string colorString)
        {
            var drawingColor = System.Drawing.ColorTranslator.FromHtml(colorString);
            return Windows.UI.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
        }

        private void AddToCartButton_Checked(object sender, RoutedEventArgs e)
        {

            try
            {
                cartService.AddToCart(ViewModel.ProductID, int.Parse(ViewModel.SelectedQuantity));
                AddToCartSuccessMessage();
            }
            catch (Exception ex)
            {
                var message = new ContentDialog()
                {
                    Title = "Error",
                    Content = $"Could not add to Cart: {ex.Message}",
                    CloseButtonText = "OK"
                };
                message.XamlRoot = this.XamlRoot;
                _ = message.ShowAsync();
            }


        }

        public void AddToCartSuccessMessage()
        {
            //message with added succesfully
            var message = new ContentDialog()
            {
                Title = "Product added to cart",
                Content = "The product has been added to your cart",
                CloseButtonText = "Ok"
            };
            //show this message
            message.XamlRoot = this.XamlRoot;
            _ = message.ShowAsync();
        }
        public void AddToCartError()
        {
            //message with error
            var message = new ContentDialog()
            {
                Title = "Error",
                Content = "The product could not be added to your cart",
                CloseButtonText = "Ok"
            };
            message.XamlRoot = this.XamlRoot;
            _ = message.ShowAsync();
        }

        public void AddToWishListButton_Checked(object sender, RoutedEventArgs e)
        {
            // Add the product to the wishlist
            // For example, add the product to a collection or database
            // Example: Wishlist.Add(product);
            // Show a success message
            try
            {
                //wishlistService.addToWishlist(ViewModel.ProductID);
                AddToWishListSuccessMessage();
            }
            catch (Exception ex)
            {
                var message = new ContentDialog()
                {
                    Title = "Error",
                    Content = $"Could not add to wishlist: {ex.Message}",
                    CloseButtonText = "OK"
                };
                message.XamlRoot = this.XamlRoot;
                _ = message.ShowAsync();
            }
        }

        public void AddToWishListSuccessMessage()
        {
            //message with added succesfully
            var message = new ContentDialog()
            {
                Title = "Product added to wishlist",
                Content = "The product has been added to your wishlist",
                CloseButtonText = "Ok"
            };
            message.XamlRoot = this.XamlRoot;
            _ = message.ShowAsync();
        }

        public void AddToWishListError()
        {
            //message with error
            var message = new ContentDialog()
            {
                Title = "Error",
                Content = "The product could not be added to your wishlist",
                CloseButtonText = "Ok"
            };
            message.XamlRoot = this.XamlRoot;
            _ = message.ShowAsync();
        }

        private void UpdateButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateWindow updateWindow = new UpdateWindow(this.product);
            updateWindow.Activate();
            //uncheck the button
            ((ToggleButton)sender).IsChecked = false;
            parent.Close();
        }

        private async void DeleteButton_Checked(object sender, RoutedEventArgs e)
        {
            var deleteConfirmationDialog = new ContentDialog()
            {
                Title = "Confirm Delete",
                Content = "Are you sure you want to delete this product?",
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await deleteConfirmationDialog.ShowAsync(); // Wait for user response

            if (result == ContentDialogResult.Primary)
            {

                productService.DeleteProduct(ViewModel.ProductID);
                // Show success message
                var successDialog = new ContentDialog()
                {
                    Title = "Product deleted",
                    Content = "The product has been deleted",
                    CloseButtonText = "Ok",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync(); // Wait before closing
            }
            MainWindow main = new MainWindow();
            parent.Close();
            main.Activate();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            parent.Close();
            main.Activate();
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            string ProductName = textBlock.Text;

            ProductRepository productRepository = new ProductRepository();
            parent.Content = new ProductTab(productRepository.GetAll().Where(p => p.Name.Equals(ProductName)).First(), parent);
        }
    }
}
