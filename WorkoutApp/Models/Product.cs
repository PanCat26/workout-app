// <copyright file="Product.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a general product with optional attributes such as colors, sizes, and weights.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the unique identifier for the product.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price of the product as a string.
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Gets or sets the file URL associated with the product.
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Gets or sets the type of the product.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product as a string.
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the list of available colors for the product.
        /// </summary>
        public List<string>? Colors { get; set; }

        /// <summary>
        /// Gets or sets the list of available sizes for the product.
        /// </summary>
        public List<string>? Sizes { get; set; }

        /// <summary>
        /// Gets or sets the list of available weights for the product.
        /// </summary>
        public List<string>? Weights { get; set; }
    }
}
