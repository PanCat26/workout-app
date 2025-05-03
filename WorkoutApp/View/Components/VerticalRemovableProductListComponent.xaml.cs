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
        public event EventHandler<int> ProductClicked;

        /// <summary>
        /// Occurs when a product is requested to be removed.
        /// </summary>
        public event EventHandler<int> ProductRemoved;

        /// <summary>
        /// Gets or sets the list of products to display.
        /// </summary>
        public IEnumerable<Product> ProductList { get; set; }

        /// <summary>
        /// Sets the product list and refreshes the view.
        /// </summary>
        /// <param name="products">The list of products to display.</param>
        public void SetProducts(IEnumerable<Product> products)
        {
            this.ProductList = products;
            this.ProductListView.ItemsSource = this.ProductList;
        }

        /// <summary>
        /// Handles click events on product items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the item click event.</param>
        public void ProductList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Product product && product.ID.HasValue)
            {
                this.ProductClicked?.Invoke(this, product.ID.Value);
            }
        }

        /// <summary>
        /// Handles click events on remove button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data for the item click event.</param>
        private void RemoveButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int productId)
            {
                this.ProductRemoved?.Invoke(this, productId);
            }
        }
    }
}
