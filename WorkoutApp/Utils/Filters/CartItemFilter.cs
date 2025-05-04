// <copyright file="CartItemFilter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Utils.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a filter interface that can be implemented to define filtering logic.
    /// </summary>
    public class CartItemFilter : IFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartItemFilter"/> class.
        /// </summary>
        /// <param name="productID">The ID of the product to filter by.</param>
        /// <param name="customerID">The ID of the customer to filter by.</param>
        public CartItemFilter(int? productID, int? customerID)
        {
            this.ProductID = productID;
            this.CustomerID = customerID;
        }

        /// <summary>
        /// Gets or sets product id to filter by.
        /// </summary>
        public int? ProductID { get; set; }

        /// <summary>
        /// Gets or sets customer id to filter by.
        /// </summary>
        public int? CustomerID { get; set; }
    }
}
