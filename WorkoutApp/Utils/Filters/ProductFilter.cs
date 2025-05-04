// <copyright file="ProductFilter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Utils.Filters
{
    /// <summary>
    /// Filter criteria for querying products.
    /// </summary>
    public class ProductFilter : IFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilter"/> class.
        /// </summary>
        /// <param name="categoryId">The category ID to filter products by.</param>
        /// <param name="excludeProductId">The product ID to exclude from the results.</param>
        /// <param name="count">The maximum number of products to return.</param>
        /// <param name="color">The color to filter products by.</param>
        /// <param name="size">The size to filter products by.</param>
        /// <param name="searchTerm">The search term to filter products by.</param>
        public ProductFilter(int? categoryId, int? excludeProductId, int? count, string? color, string? size, string? searchTerm)
        {
            this.CategoryId = categoryId;
            this.ExcludeProductId = excludeProductId;
            this.Count = count;
            this.Color = color;
            this.Size = size;
            this.SearchTerm = searchTerm;
        }

        /// <summary>
        /// Gets or sets the category ID to filter products by.
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the product ID to exclude from the results.
        /// </summary>
        public int? ExcludeProductId { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of products to return.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Gets or sets the color to filter products by.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the size to filter products by.
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Gets or sets the search term to filter products by.
        /// </summary>
        public string? SearchTerm { get; set; }
    }
}
