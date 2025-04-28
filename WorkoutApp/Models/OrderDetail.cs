// <copyright file="OrderDetail.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents a detail line item within an order.
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetail"/> class.
        /// </summary>
        /// <param name="iD">The unique identifier for the order detail.</param>
        /// <param name="orderID">The ID of the associated order.</param>
        /// <param name="productID">The ID of the product in the order.</param>
        /// <param name="quantity">The quantity of the product ordered.</param>
        /// <param name="price">The price of the product at the time of order.</param>
        /// <param name="isActive">A value indicating whether the order detail is active.</param>
        public OrderDetail(int iD, int orderID, int productID, int quantity, double price, bool isActive)
        {
            this.ID = iD;
            this.OrderID = orderID;
            this.ProductID = productID;
            this.Quantity = quantity;
            this.Price = price;
            this.IsActive = isActive;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the order detail.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the order this detail belongs to.
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the product in the order detail.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product ordered.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price of the product at the time of the order.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order detail is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
