// <copyright file="ProductFilter.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
// </copyright>

namespace WorkoutApp.Utils.Filters
{
    /// <summary>
    /// Filter criteria for querying products.
    /// </summary>
    public class ProductFilter(int? categoryId, int? excludeProductId, int? count) : IFilter // Added constructor
    {
        /// <summary>
        /// Gets or sets the category ID to filter products by.
        /// </summary>
        public int? CategoryId { get; set; } = categoryId; // Initialize from constructor

        /// <summary>
        /// Gets or sets the product ID to exclude from the results.
        /// </summary>
        public int? ExcludeProductId { get; set; } = excludeProductId; // Initialize from constructor

        /// <summary>
        /// Gets or sets the maximum number of products to return.
        /// </summary>
        public int? Count { get; set; } = count; // Initialize from constructor

        // You can add other filter properties here (e.g., min/max price, search term, etc.)
    }
}
