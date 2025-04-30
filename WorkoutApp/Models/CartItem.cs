// <copyright file="CartItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an item inside a shopping cart.
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartItem"/> class.
        /// </summary>
        /// <param name="customerId">The unique identifier for the customer.</param>
        /// <param name="productId">The unique identifier for the product.</param>
        /// <param name="quantity">The quantity of the item.</param>
        public CartItem(int customerId, int productId, int quantity)
        {
            this.CustomerId = customerId;
            this.ProductId = productId;
            this.Quantity = quantity;
        }

        /// <summary>
        /// Gets or sets the identifier of the customer to which this item belongs.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the product associated with this cart item.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product in the cart item.
        /// </summary>
        public int Quantity { get; set; }
    }
}
