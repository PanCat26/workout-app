// <copyright file="CartItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an item in a shopping cart.
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Gets or sets the ID of the order.
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// Gets or sets the product ID associated with the cart item.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Gets or sets the customer ID associated with the cart item.
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product in the cart.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItem"/> class.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <param name="productID">The ID of the product associated with the cart item.</param>
        /// <param name="customerID">The ID of the customer associated with the cart item.</param>
        /// <param name="quantity">The quantity of the product in the cart.</param>
        public CartItem(int? id, int productID, int customerID, int quantity)
        {
            this.ID = id;
            this.ProductID = productID;
            this.CustomerID = customerID;
            this.Quantity = quantity;
        }

        public CartItem(int productID, int customerID, int quantity)
        {
            ProductID = productID;
            CustomerID = customerID;
            Quantity = quantity;
        }
    }
}
