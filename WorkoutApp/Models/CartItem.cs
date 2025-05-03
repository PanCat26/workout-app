// <copyright file="CartItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an item in a shopping cart.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CartItem"/> class.
    /// </remarks>
    /// <param name="product">The product associated with the cart item.</param>
    /// <param name="customerID">The ID of the customer associated with the cart item.</param>
    public class CartItem(int? id, Product product, int customerID)
    {
        /// <summary>
        /// Gets or sets the ID of the order.
        /// </summary>
        public int? ID { get; set; } = id;

        /// <summary>
        /// Gets or sets the product ID associated with the cart item.
        /// </summary>
        public Product Product { get; set; } = product;

        /// <summary>
        /// Gets or sets the customer ID associated with the cart item.
        /// </summary>
        public int CustomerID { get; set; } = customerID;
    }
}
