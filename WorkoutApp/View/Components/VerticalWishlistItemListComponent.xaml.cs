// <copyright file="VerticalWishlistItemListComponent.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View.Components
{
    using System;
    using System.Collections.Generic;
    using Microsoft.UI.Xaml.Controls;
    using WorkoutApp.Models;

    /// <summary>
    /// Represents a component that displays a vertical list of wishlist items.
    /// </summary>
    public sealed partial class VerticalWishlistItemListComponent : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerticalWishlistItemListComponent"/> class.
        /// </summary>
        public VerticalWishlistItemListComponent()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Occurs when a product is clicked.
        /// </summary>
        public event EventHandler<int> WishlistItemClicked;

        /// <summary>
        /// Occurs when a product is requested to be removed.
        /// </summary>
        public event EventHandler<int> WishlistItemRemoved;

        /// <summary>
        /// Gets or sets the list of products to display.
        /// </summary>
        public IEnumerable<WishlistItem> WishlistItemList { get; set; }

        /// <summary>
        /// Sets the product list and refreshes the view.
        /// </summary>
        /// <param name="wishlistItems">The list of products to display.</param>
        public void SetProducts(IEnumerable<WishlistItem> wishlistItems)
        {
            this.WishlistItemList = wishlistItems;
            this.ProductListView.ItemsSource = this.WishlistItemList;
        }

        /// <summary>
        /// Handles click events on product items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the item click event.</param>
        public void ProductList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is WishlistItem wishlistItem && wishlistItem.Product.ID.HasValue)
            {
                this.WishlistItemClicked?.Invoke(this, wishlistItem.Product.ID.Value);
            }
        }

        private async void RemoveButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Confirm Removal",
                Content = "Are you sure you want to remove this item from your wishlist?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot, // Required in WinUI 3
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (sender is Button button && button.Tag is int wishlistItemId)
                {
                    this.WishlistItemRemoved?.Invoke(this, wishlistItemId);
                }
            }
        }
    }
}
