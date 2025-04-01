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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View.ProductTab
{
    public sealed partial class ProductTab : UserControl
    {
        public ProductDetailsViewModel ViewModel { get; set; } = new ProductDetailsViewModel();

        public ProductTab()
        {
            this.InitializeComponent();

            DataContext = ViewModel;
            PopulateColorFlyout();
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
            // Add the product to the cart
            // For example, add the product to a collection or database
            // Example: Cart.Add(product);
            // Show a success message
            AddToCartSuccessMessage();

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
            AddToWishListSuccessMessage();
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
        private void View_Button(object sender, RoutedEventArgs e)
        {
            // Show the product details
            //navigate to a new ProductPage containing the product details
            // For example, navigate to a new page with the product details


        }

        private void UpdateButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateWindow updateWindow = new UpdateWindow();
            updateWindow.Activate();
            //uncheck the button
            ((ToggleButton)sender).IsChecked = false;
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
                // Delete the product
                // Example: ProductCollection.Remove(product);

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
        }

    }
}
