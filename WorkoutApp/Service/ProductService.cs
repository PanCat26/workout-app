// <copyright file="ProductService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;

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
            return await this.productRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<Product> GetByIdAsync(int id)
        {
            return await this.productRepository.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Product> UpdateAsync(Product entity)
        {
            return await this.productRepository.UpdateAsync(entity);
        }
    }
}
