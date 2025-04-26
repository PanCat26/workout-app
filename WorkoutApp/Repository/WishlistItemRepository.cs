// <copyright file="WishlistItemRepository.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
// </copyright>
namespace WorkoutApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using WorkoutApp.Models;

    /// <summary>
    /// Repository class for managing Wishlist items in the database.
    /// Implements the <see cref="IRepository{WishlistItem}"/> interface.
    /// </summary>
    public class WishlistItemRepository : IRepository<WishlistItem>
    {
        private readonly string connectionString = @"Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistItemRepository"/> class.
        /// Opens a new database connection.
        /// </summary>
        public WishlistItemRepository()
        {
            this.connection = new SqlConnection(this.connectionString);
        }

        /// <summary>
        /// Retrieves all active wishlist items for the active customer asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="WishlistItem"/> objects.</returns>
        public async Task<IEnumerable<WishlistItem>> GetAllAsync()
        {
            this.connection.Open();
            var wishListItems = new List<WishlistItem>();

            int customerId = this.GetActiveCustomerId();

            var selectCommand = new SqlCommand(
                "SELECT * FROM Wishlist WHERE IsActive = 1 AND CustomerID = @CustomerID",
                this.connection);
            selectCommand.Parameters.AddWithValue("@CustomerID", customerId);

            var reader = await selectCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new WishlistItem
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                };

                wishListItems.Add(item);
            }

            reader.Close();
            this.connection.Close();

            return wishListItems;
        }

        /// <summary>
        /// Retrieves a wishlist item by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the wishlist item.</param>
        /// <returns>A <see cref="WishlistItem"/> object if found, otherwise <c>null</c>.</returns>
        public async Task<WishlistItem> GetByIdAsync(long id)
        {
            this.connection.Open();

            var selectCommand = new SqlCommand(
                "SELECT * FROM Wishlist WHERE IsActive = 1 AND ID = @ID",
                this.connection);
            selectCommand.Parameters.AddWithValue("@ID", id);

            var reader = await selectCommand.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                reader.Close();
                this.connection.Close();
                return null;
            }

            var item = new WishlistItem
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
            };

            reader.Close();
            this.connection.Close();

            return item;
        }

        /// <summary>
        /// Creates a new wishlist item in the database asynchronously.
        /// </summary>
        /// <param name="entity">The <see cref="WishlistItem"/> to create.</param>
        /// <returns>The created <see cref="WishlistItem"/> object.</returns>
        public async Task<WishlistItem> CreateAsync(WishlistItem entity)
        {
            this.connection.Open();

            var insertCommand = new SqlCommand(
                "INSERT INTO Wishlist (ID, ProductID, CustomerID, IsActive) VALUES (@ID, @ProductID, @CustomerID, @IsActive)",
                this.connection);
            var getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(ID), 0) + 1 FROM Wishlist", this.connection);
            int newId = (int)await getMaxIdCommand.ExecuteScalarAsync();

            insertCommand.Parameters.AddWithValue("@ID", newId);
            insertCommand.Parameters.AddWithValue("@ProductID", entity.ProductID);
            insertCommand.Parameters.AddWithValue("@CustomerID", this.GetActiveCustomerId());
            insertCommand.Parameters.AddWithValue("@IsActive", 1);

            await insertCommand.ExecuteNonQueryAsync();

            this.connection.Close();
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
            return entity;
        }

        /// <summary>
        /// Deletes a wishlist item by its ID asynchronously.
        /// Marks the item as inactive instead of actually deleting it.
        /// </summary>
        /// <param name="id">The ID of the wishlist item to delete.</param>
        /// <returns><c>true</c> if the item was deleted successfully, otherwise <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(long id)
        {
            this.connection.Open();

            var updateCommand = new SqlCommand(
                "UPDATE Wishlist SET IsActive = 0 WHERE ID = @ID",
                this.connection);
            updateCommand.Parameters.AddWithValue("@ID", id);

            await updateCommand.ExecuteNonQueryAsync();

            this.connection.Close();
            return true;
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
