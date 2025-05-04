// <copyright file="ProductService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;
    using WorkoutApp.Utils.Filters; // Assuming IFilter and ProductFilter are here

    /// <summary>
    /// Service class for handling product operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ProductService"/> class.
    /// </remarks>
    /// <param name="productRepository">The product repository.</param>
    public class ProductService(IRepository<Product> productRepository) : IService<Product>
    {
        private readonly IRepository<Product> productRepository = productRepository;

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
            return (await this.productRepository.GetByIdAsync(id)) !;
        }

        /// <inheritdoc/>
        public async Task<Product> UpdateAsync(Product entity)
        {
            // Calling the generic UpdateAsync from the repository
            return await this.productRepository.UpdateAsync(entity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetFilteredAsync(IFilter filter)
        {
            // Call the repository's GetAllFilteredAsync method with the filter
            // The repository handles the actual filtering logic (SQL query)
            return await this.productRepository.GetAllFilteredAsync(filter);
        }
    }
}
