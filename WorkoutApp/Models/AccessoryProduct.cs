// <copyright file="AccessoryProduct.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an accessory product in the Workout App.
    /// </summary>
    public class AccessoryProduct : IProduct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessoryProduct"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the product.</param>
        /// <param name="name">The name of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="stock">The available stock quantity of the product.</param>
        /// <param name="categoryId">The category ID the product belongs to.</param>
        /// <param name="description">A brief description of the product.</param>
        /// <param name="fileUrl">The URL for the file associated with the product (e.g., an image).</param>
        /// <param name="isActive">A value indicating whether the product is active.</param>
        public AccessoryProduct(int id, string name, double price, int stock, int categoryId, string description, string fileUrl, bool isActive)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Stock = stock;
            this.CategoryID = categoryId;
            this.Description = description;
            this.FileUrl = fileUrl;
            this.IsActive = isActive;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the accessory product.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the accessory product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price of the accessory product.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity available for the accessory product.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Gets or sets the category ID associated with the accessory product.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the description of the accessory product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the file URL (e.g., image or document) related to the accessory product.
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the accessory product is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
