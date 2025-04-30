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
        ///  Initializes a new instance of the <see cref="WishlistItem"/> class.
        /// </summary>
        /// <param name="customerId">The unique identifier for the cusotmer.</param>
        /// <param name="productId">The unique identifier for the product.</param>
        public WishlistItem(int customerId, int productId)
        {
            this.CustomerID = customerId;
            this.ProductID = productId;
        }

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
