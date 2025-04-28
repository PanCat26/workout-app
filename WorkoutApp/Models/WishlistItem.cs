// <copyright file="WishlistItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an item added to a customer's wishlist.
    /// </summary>
    public class WishlistItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the wishlist item.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the product associated with this wishlist item.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the customer who added this item to their wishlist.
        /// </summary>
        public int CustomerID { get; set; }
    }
}
