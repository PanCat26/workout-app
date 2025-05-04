// <copyright file="VerticalProductListComponent.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View.Components
{
    using System;
    using System.Collections.Generic;
    using Microsoft.UI.Xaml.Controls;
    using WorkoutApp.Models;

    /// <summary>
    /// A reusable component that displays a vertical list of products.
    /// </summary>
    public sealed partial class VerticalRemovableProductListComponent : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerticalRemovableProductListComponent"/> class.
        /// </summary>
        public VerticalRemovableProductListComponent()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Occurs when a product is clicked.
        /// </summary>
        public event EventHandler<int> CartItemClicked;

        /// <summary>
        /// Occurs when a product is requested to be removed.
        /// </summary>
        public event EventHandler<int> CartItemRemoved;

        /// <summary>
        /// Gets or sets the list of products to display.
        /// </summary>
        public IEnumerable<CartItem> CartItemList { get; set; }

        /// <summary>
        /// Sets the product list and refreshes the view.
        /// </summary>
        /// <param name="cartItems">The list of products to display.</param>
        public void SetProducts(IEnumerable<CartItem> cartItems)
        {
            this.CartItemList = cartItems;
            this.ProductListView.ItemsSource = this.CartItemList;
        }

        /// <summary>
        /// Handles click events on product items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the item click event.</param>
        public void ProductList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is CartItem cartItem && cartItem.Product.ID.HasValue)
            {
                this.CartItemClicked?.Invoke(this, cartItem.Product.ID.Value);
            }
        }

        /// <summary>
        /// Handles click events on remove button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the item click event.</param>
        private void RemoveButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int cartItemId)
            {
                this.CartItemClicked?.Invoke(this, cartItemId);
            }
        }
    }
}
