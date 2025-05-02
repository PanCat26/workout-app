// <copyright file="WishlistRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Infrastructure.Session;
    using WorkoutApp.Models;

    /// <summary>
    /// Provides CRUD operations for wishlist items in the database.
    /// </summary>
    public class WishlistRepository : IRepository<WishlistItem>
    {
        private readonly DbService databaseService;
        private readonly SessionManager sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service used for executing queries.</param>
        /// <param name="sessionManager">The session manager for managing user sessions.</param>
        public WishlistRepository(DbService dbService, SessionManager sessionManager)
        {
            this.databaseService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            this.sessionManager = sessionManager;
        }

        /// <summary>
        /// Retrieves all wishlist items asynchronously.
        /// </summary>
        /// <returns>A collection of wishlist items.</returns>
        public async Task<IEnumerable<WishlistItem>> GetAllAsync()
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");
            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            List<WishlistItem> products = new List<WishlistItem>();

            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT wi.ID, " +
                    "wi.ProductID, " +
                    "wi.CustomerID " +
                    "FROM WishlistItem wi " +
                    "WHERE wi.CustomerID = @CustomerID;",
                    new List<SqlParameter> { new SqlParameter("@CustomerID", customerID) });

                foreach (DataRow row in selectQueryResult.Rows)
                {
                    WishlistItem wishlistItem = new WishlistItem
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        CustomerID = Convert.ToInt32(row["CustomerID"])
                    };
                    products.Add(wishlistItem);
                }

                return products;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving wishlist items: {exception.Message}");
            }
        }

        /// <summary>
        /// Retrieves a wishlist item by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the wishlist item.</param>
        /// <returns>The wishlist item with the specified ID, or null if not found.</returns>
        public async Task<WishlistItem?> GetByIdAsync(int id)
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            if (id <= 0)
            {
                throw new ArgumentException("Invalid product ID.");
            }

            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT wi.ID, " +
                    "wi.ProductID, " +
                    "wi.CustomerID " +
                    "FROM WishlistItem wi " +
                    "WHERE wi.CustomerID = @CustomerID AND wi.ID = @ID;",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@CustomerID", customerID),
                        new SqlParameter("@ID", id),
                    });

                if (selectQueryResult.Rows.Count == 0)
                {
                    return null;
                }

                DataRow row = selectQueryResult.Rows[0];

                WishlistItem wishlistItem = new WishlistItem
                {
                    ID = Convert.ToInt32(row["ID"]),
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    CustomerID = Convert.ToInt32(row["CustomerID"])
                };

                return wishlistItem;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving wishlist item: {exception.Message}");
            }
        }

        /// <summary>
        /// Creates a new wishlist item asynchronously.
        /// </summary>
        /// <param name="entity">The wishlist item to create.</param>
        /// <returns>The created wishlist item.</returns>
        public async Task<WishlistItem> CreateAsync(WishlistItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            if (entity.ProductID <= 0)
            {
                throw new ArgumentException("Invalid product ID.");
            }

            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            int insertQueryResult = await this.databaseService.ExecuteQueryAsync(
                "INSERT INTO WishlistItem (ProductID, CustomerID) VALUES (@ProductID, @CustomerID)",
                new List<SqlParameter>
                {
                    new SqlParameter("@ProductID", entity.ProductID),
                    new SqlParameter("@CustomerID", customerID),
                });

            if (insertQueryResult < 0)
            {
                throw new Exception($"Error inserting wishlist item with product id: {entity.ProductID}");
            }

            return entity;
        }

        /// <summary>
        /// Updates an existing wishlist item asynchronously.
        /// </summary>
        /// <param name="wishlistItem">The wishlist item to update.</param>
        /// <returns>The updated wishlist item.</returns>
        public async Task<WishlistItem> UpdateAsync(WishlistItem wishlistItem)
        {
            if (wishlistItem == null)
            {
                throw new ArgumentNullException(nameof(wishlistItem));
            }

            if (wishlistItem.ProductID <= 0)
            {
                throw new ArgumentException("Invalid product ID.");
            }

            if (wishlistItem.CustomerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            // For wishlist items, we don't need to update anything as they only contain product and customer IDs
            return wishlistItem;
        }

        /// <summary>
        /// Deletes a wishlist item asynchronously.
        /// </summary>
        /// <param name="id">The ID of the product to remove from wishlist.</param>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            if (id <= 0)
            {
                throw new ArgumentException("Invalid product ID.");
            }

            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            int deleteQueryResult = await this.databaseService.ExecuteQueryAsync(
                "DELETE FROM WishlistItem WHERE ID = @ID AND CustomerID = @CustomerID",
                new List<SqlParameter>
                {
                    new SqlParameter("@ID", id),
                    new SqlParameter("@CustomerID", customerID),
                });

            return deleteQueryResult > 0;
        }
    }
} 