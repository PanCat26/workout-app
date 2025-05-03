// <copyright file="WishlistItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an item in a customer's wishlist.
    /// </summary>
    public class WishlistItem(int? id, Product product, int customerID)
    {
        /// <summary>
        /// Gets or sets the unique identifier for the wishlist item.
        /// </summary>
        public int? ID { get; set; } = id;

        /// <summary>
        /// Gets or sets the product associated with this wishlist item.
        /// </summary>
        public Product Product { get; set; } = product;

        /// <summary>
        /// Gets or sets the identifier of the customer who owns this wishlist item.
        /// </summary>
        public int CustomerID { get; set; } = customerID;
    }
}
