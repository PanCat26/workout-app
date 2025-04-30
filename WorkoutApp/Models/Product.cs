// <copyright file="Product.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    /// <summary>
    /// Represents an accessory product in the Workout App.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the product.</param>
        /// <param name="name">The name of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="stock">The available stock quantity of the product.</param>
        /// <param name="category">The category the product belongs to.</param>
        /// <param name="description">A brief description of the product.</param>
        /// <param name="photoURL">The URL for the photo associated with the product.</param>
        public Product(int id, string name, decimal price, int stock, Category category, string description, string photoURL)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Stock = stock;
            this.Category = category;
            this.Description = description;
            this.PhotoURL = photoURL;
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
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity available for the accessory product.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Gets or sets the category associated with the accessory product.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Gets or sets the description of the accessory product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the photo URL related to the accessory product.
        /// </summary>
        public string PhotoURL { get; set; }
    }
}
