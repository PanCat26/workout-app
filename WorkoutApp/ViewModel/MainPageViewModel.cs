// <copyright file="MainPageViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;
    using WorkoutApp.Service;
    using WorkoutApp.Utils.Filters;

    /// <summary>
    /// The view model for the main page, responsible for loading and providing product data.
    /// </summary>
    public class MainPageViewModel
    {
        private readonly IService<Product> productService;
        private ProductFilter filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// Sets up the database connection and product service.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the database connection string is missing or empty.
        /// </exception>
        public MainPageViewModel()
        {
            string? connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string 'DefaultConnection' is not configured or is null.");
            }

            var dbConnectionFactory = new DbConnectionFactory(connectionString);
            var dbService = new DbService(dbConnectionFactory);
            IRepository<Product> productRepository = new ProductRepository(dbService);
            this.productService = new ProductService(productRepository);
            this.filter = new ProductFilter(null, null, null);
        }

        /// <summary>
        /// Retrieves all products asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of products.</returns>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await ((ProductService)this.productService).GetFilteredAsync(this.filter);
        }

        public void SetSelectedCategoryID(int categoryId)
        {
            this.filter.CategoryId = categoryId;
        }


    }
}
