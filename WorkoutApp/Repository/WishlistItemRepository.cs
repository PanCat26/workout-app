// <copyright file="WishlistItemRepository.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
// </copyright>
namespace WorkoutApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Models;

    /// <summary>
    /// Repository class for managing Wishlist items in the database.
    /// Implements the <see cref="IWishlistItemRepository"/> interface.
    /// </summary>
    public class WishlistItemRepository : IRepository<WishlistItem>
    {
        private readonly DbService dbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistItemRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service.</param>
        public WishlistItemRepository(DbService dbService)
        {
            this.dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        /// <summary>
        /// Retrieves all active wishlist items for the active customer.
        /// </summary>
        /// <returns>A list of <see cref="WishlistItem"/> objects.</returns>
        public List<WishlistItem> GetAll()
        {
            var task = Task.Run(async () => await this.GetAllAsync());
            return new List<WishlistItem>(task.Result);
        }

        /// <summary>
        /// Retrieves all active wishlist items for the active customer asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="WishlistItem"/> objects.</returns>
        public async Task<IEnumerable<WishlistItem>> GetAllAsync()
        {
            int customerId = this.GetActiveCustomerId();

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerID", customerId)
            };

            var query = "SELECT * FROM Wishlist WHERE IsActive = 1 AND CustomerID = @CustomerID";

            DataTable dataTable = await this.dbService.ExecuteSelectAsync(query, parameters);
            var wishListItems = new List<WishlistItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                var item = new WishlistItem
                {
                    ID = Convert.ToInt32(row["ID"]),
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    CustomerID = Convert.ToInt32(row["CustomerID"]),
                };

                wishListItems.Add(item);
            }

            return wishListItems;
        }

        /// <summary>
        /// Retrieves a wishlist item by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the wishlist item.</param>
        /// <returns>A <see cref="WishlistItem"/> object if found, otherwise <c>null</c>.</returns>
        public async Task<WishlistItem> GetByIdAsync(long id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };

            var query = "SELECT * FROM Wishlist WHERE IsActive = 1 AND ID = @ID";

            DataTable dataTable = await this.dbService.ExecuteSelectAsync(query, parameters);

            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dataTable.Rows[0];
            var item = new WishlistItem
            {
                ID = Convert.ToInt32(row["ID"]),
                ProductID = Convert.ToInt32(row["ProductID"]),
                CustomerID = Convert.ToInt32(row["CustomerID"]),
            };

            return item;
        }

        /// <summary>
        /// Creates a new wishlist item in the database asynchronously.
        /// </summary>
        /// <param name="entity">The <see cref="WishlistItem"/> to create.</param>
        /// <returns>The created <see cref="WishlistItem"/> object.</returns>
        public async Task<WishlistItem> CreateAsync(WishlistItem entity)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProductID", entity.ProductID),
                new SqlParameter("@CustomerID", this.GetActiveCustomerId()),
                new SqlParameter("@IsActive", 1)
            };

            string query = @"
            INSERT INTO Wishlist (ProductID, CustomerID, IsActive)
            OUTPUT INSERTED.ID
            VALUES (@ProductID, @CustomerID, @IsActive);";

            var resultTable = await this.dbService.ExecuteSelectAsync(query, parameters);

            entity.ID = Convert.ToInt32(resultTable.Rows[0][0]);
            entity.CustomerID = this.GetActiveCustomerId();

            return entity;
        }


        /// <summary>
        /// Updates an existing wishlist item.
        /// This functionality is not supported for wishlist items, so it returns the entity unchanged.
        /// </summary>
        /// <param name="entity">The <see cref="WishlistItem"/> to update.</param>
        /// <returns>The original <see cref="WishlistItem"/> object.</returns>
        public async Task<WishlistItem> UpdateAsync(WishlistItem entity)
        {
            // Wishlist items don't have an update functionality.
            return await Task.FromResult(entity);
        }

        /// <summary>
        /// Deletes a wishlist item by its ID asynchronously.
        /// Marks the item as inactive instead of actually deleting it.
        /// </summary>
        /// <param name="id">The ID of the wishlist item to delete.</param>
        /// <returns><c>true</c> if the item was deleted successfully, otherwise <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(long id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };

            var query = "UPDATE Wishlist SET IsActive = 0 WHERE ID = @ID";

            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);

            return affectedRows > 0;
        }

        /// <summary>
        /// Adds a new product to the wishlist.
        /// </summary>
        /// <param name="productId">The ID of the product to add to the wishlist.</param>
        /// <returns>The added <see cref="WishlistItem"/> object.</returns>
        public WishlistItem AddWishlistItem(int productId)
        {
            var wishlistItem = new WishlistItem
            {
                ProductID = productId,
                CustomerID = this.GetActiveCustomerId()
            };

            var task = Task.Run(async () => await this.CreateAsync(wishlistItem));
            return task.Result;
        }

        /// <summary>
        /// Deletes a wishlist item by its ID.
        /// </summary>
        /// <param name="id">The ID of the wishlist item to delete.</param>
        /// <returns><c>true</c> if the item was deleted successfully, otherwise <c>false</c>.</returns>
        public bool DeleteById(long id)
        {
            var task = Task.Run(async () => await this.DeleteAsync(id));
            return task.Result;
        }

        /// <summary>
        /// Gets the active customer ID.
        /// </summary>
        /// <returns>The active customer's ID.</returns>
        private int GetActiveCustomerId()
        {
            return 1; // In a real-world application, this should be dynamic, based on the currently logged-in user.
        }
    }
}