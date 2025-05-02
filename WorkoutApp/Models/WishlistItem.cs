// <copyright file="WishlistItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an item in a customer's wishlist.
    /// </summary>
    public class WishlistItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistItem"/> class.
        /// </summary>
        /// <param name="id">The ID of the wishlist item.</param>
        /// <param name="product">The product in the wishlist.</param>
        /// <param name="customerID">The ID of the customer who owns the wishlist.</param>

        public WishlistItem(int? id, Product product, int customerID)
        {
            this.ID = id;
            this.Product = product ?? throw new ArgumentNullException(nameof(product));
            this.CustomerID = customerID;
        }

        public WishlistItem() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistItem"/> class.
        /// </summary>
        /// <param name="product">The product in the wishlist.</param>
        /// <param name="customerID">The ID of the customer who owns the wishlist.</param>
        public WishlistItem(Product product, int customerID)
            : this(null, product, customerID)
        {
        }

        /// <summary>
        /// Gets or sets the ID of the wishlist item.
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// Gets or sets the product in the wishlist.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the ID of the customer who owns the wishlist.
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        public int ProductID { get; set; }
    }
} 