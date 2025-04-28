// <copyright file="Order.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    using System;

    /// <summary>
    /// Represents a customer's order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the customer who placed the order.
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// Gets or sets the date the order was placed.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the total amount of the order.
        /// </summary>
        public double TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
