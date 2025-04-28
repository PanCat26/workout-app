// <copyright file="IProduct.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Defines the common properties of a product in the shop.
    /// </summary>
    public interface IProduct
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        double Price { get; set; }

        /// <summary>
        /// Gets or sets the available stock quantity of the product.
        /// </summary>
        int Stock { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the category to which the product belongs.
        /// </summary>
        int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of the image or file associated with the product.
        /// </summary>
        string FileUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is active.
        /// </summary>
        bool IsActive { get; set; }
    }
}
