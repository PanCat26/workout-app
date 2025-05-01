// <copyright file="OrderItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an item within an order, including the product and its quantity.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderItem"/> class.
        /// </summary>
        /// <param name="product">The product in the order.</param>
        /// <param name="quantity">The quantity of the product.</param>
        public OrderItem(Product product, int quantity)
        {
            this.Product = product;
            this.Quantity = quantity;
        }

        /// <summary>
        /// Gets or sets the product in the order item.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product.
        /// </summary>
        public int Quantity { get; set; }
    }
}
