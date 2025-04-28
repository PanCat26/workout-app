// <copyright file="ClothesProduct.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents a clothes product in the shop.
    /// </summary>
    public class ClothesProduct : IProduct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClothesProduct"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="name">The name of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="stock">The stock quantity of the product.</param>
        /// <param name="categoryId">The category identifier of the product.</param>
        /// <param name="color">The color or attribute of the product.</param>
        /// <param name="size">The size of the clothes product.</param>
        /// <param name="description">The description of the product.</param>
        /// <param name="fileUrl">The file URL for the product image.</param>
        /// <param name="isActive">Indicates whether the product is active.</param>
        public ClothesProduct(
            int id,
            string name,
            double price,
            int stock,
            int categoryId,
            string color,
            string size,
            string description,
            string fileUrl,
            bool isActive)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Stock = stock;
            this.CategoryID = categoryId;
            this.Attributes = color;
            this.Size = size;
            this.Description = description;
            this.FileUrl = fileUrl;
            this.IsActive = isActive;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity available for the product.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Gets or sets the category identifier for the product.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the attributes (e.g., color) of the product.
        /// </summary>
        public string Attributes { get; set; }

        /// <summary>
        /// Gets or sets the size of the clothes product.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of the file/image associated with the product.
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
