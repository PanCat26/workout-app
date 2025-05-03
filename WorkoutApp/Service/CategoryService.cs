// <copyright file="CategoryService.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
// </copyright>

namespace WorkoutApp.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;
    using System.Linq;


    /// <summary>
    /// Service class for managing category operations.
    /// Implements the <see cref="IService{Category}"/> interface.a
    /// </summary>
    public class CategoryService : IService<Category> // Assuming IService<T> interface exists
    {
        private readonly IRepository<Category> categoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="categoryRepository">The category repository.</param>
        public CategoryService(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        /// <summary>
        /// Gets all categories asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with a collection of categories.</returns>
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var result = await this.categoryRepository.GetAllAsync();
            // Delegate the call to the repository
            System.Diagnostics.Debug.WriteLine($"[CategoryService] Fetched {result?.Count()} categories.");

            return result;
        }

        /// <summary>
        /// Gets a category by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>A task representing the asynchronous operation with the category.</returns>
        public async Task<Category?> GetByIdAsync(int id) // Using int id as per IRepository
        {
            // Delegate the call to the repository
            return await this.categoryRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Creates a new category asynchronously.
        /// </summary>
        /// <param name="entity">The category to create.</param>
        /// <returns>A task representing the asynchronous operation with the created category.</returns>
        public async Task<Category> CreateAsync(Category entity)
        {
            // Delegate the call to the repository
            return await this.categoryRepository.CreateAsync(entity);
        }

        /// <summary>
        /// Updates an existing category asynchronously.
        /// </summary>
        /// <param name="entity">The category to update.</param>
        /// <returns>A task representing the asynchronous operation with the updated category.</returns>
        public async Task<Category> UpdateAsync(Category entity)
        {
            // Delegate the call to the repository
            return await this.categoryRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// Deletes a category by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>A task representing the asynchronous operation with a boolean indicating success.</returns>
        public async Task<bool> DeleteAsync(int id) // Using int id as per IRepository
        {
            // Delegate the call to the repository
            return await this.categoryRepository.DeleteAsync(id);
        }
    }
}
