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
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="iD">The unique identifier for the order.</param>
        /// <param name="customerID">The ID of the customer who placed the order.</param>
        /// <param name="orderDate">The date the order was placed.</param>
        /// <param name="totalAmount">The total amount of the order.</param>
        /// <param name="isActive">A value indicating whether the order is active.</param>
        public Order(int iD, int customerID, DateTime orderDate, double totalAmount, bool isActive)
        {
            this.ID = iD;
            this.CustomerID = customerID;
            this.OrderDate = orderDate;
            this.TotalAmount = totalAmount;
            this.IsActive = isActive;
        }

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
