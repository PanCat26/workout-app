// <copyright file="Order.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a customer's order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <param name="orderItems">The list of items in the order.</param>
        /// <param name="orderDate">The date the order was placed.</param>
        public Order(int id, List<OrderItem> orderItems, DateTime orderDate)
        {
            this.ID = id;
            this.OrderItems = orderItems;
            this.OrderDate = orderDate;
        }

        /// <summary>
        /// Gets or sets the ID of the order.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the list of products in the order.
        /// </summary>
        public List<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// Gets or sets the date the order was placed.
        /// </summary>
        public DateTime OrderDate { get; set; }
    }
}
