// <copyright file="ProductService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Service
{
    using System.Collections.Generic;
    using System.Linq; // Required for .ToList() if needed, but not directly used in this method
    using System.Threading.Tasks;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;
    using WorkoutApp.Utils.Filters; // Assuming IFilter and ProductFilter are here

    /// <summary>
    /// Service class for handling product operations.
    /// </summary>
    public class ProductService : IService<Product>
    {
        private readonly IRepository<Product> productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="productRepository">The product repository.</param>
        public ProductService(IRepository<Product> productRepository)
        {
            this.productRepository = productRepository;
        }

        /// <inheritdoc/>
        public async Task<Product> CreateAsync(Product entity)
        {
            return await this.productRepository.CreateAsync(entity);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            return await this.productRepository.DeleteAsync(id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // Calling the generic GetAllAsync from the repository
            return await this.productRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<Product> GetByIdAsync(int id)
        {
            // Calling the generic GetByIdAsync from the repository
            return await this.productRepository.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Product> UpdateAsync(Product entity)
        {
            // Calling the generic UpdateAsync from the repository
            return await this.productRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// Gets a specified number of products from a specific category, excluding a given product ID.
        /// This method calls the repository's filtered get method.
        /// </summary>
        /// <param name="categoryId">The ID of the category to filter by.</param>
        /// <param name="excludeProductId">The ID of the product to exclude from the results.</param>
        /// <param name="count">The maximum number of products to return.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of products.</returns>
        // Implementing the GetProductsByCategoryAsync method
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, int excludeProductId, int count)
        {
            // Create a ProductFilter instance with the desired criteria
            var filter = new ProductFilter(categoryId, excludeProductId, count);

            // Call the repository's GetAllFilteredAsync method with the filter
            // The repository handles the actual filtering logic (SQL query)
            return await this.productRepository.GetAllFilteredAsync(filter);
        }
    }
}
