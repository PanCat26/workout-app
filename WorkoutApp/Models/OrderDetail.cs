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
