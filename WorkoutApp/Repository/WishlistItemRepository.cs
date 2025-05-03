// <copyright file="WishlistItemRepository.cs" company="PlaceholderCompany">
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
    public class WishlistItemRepository : IRepository<WishlistItem>
    {
        private readonly DbService databaseService;
        private readonly SessionManager sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistItemRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service used for executing queries.</param>
        /// <param name="sessionManager">The session manager for managing user sessions.</param>
        public WishlistItemRepository(DbService dbService, SessionManager sessionManager)
        {
            this.databaseService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            this.sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
        }

        /// <summary>
        /// Retrieves all wishlist items for the current user.
        /// </summary>
        /// <returns>A collection of <see cref="WishlistItem"/> objects.</returns>
        public async Task<IEnumerable<WishlistItem>> GetAllAsync()
        {
            int customerId = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            string selectQuery = @"
                SELECT wi.ID AS WishlistID, wi.CustomerID, 
                       p.ID AS ProductID, p.Name, p.Price, p.Stock, p.Size, p.Color, p.Description, p.PhotoURL,
                       c.ID AS CategoryID, c.Name AS CategoryName
                FROM WishlistItem wi
                INNER JOIN Product p ON wi.ProductID = p.ID
                INNER JOIN Category c ON p.CategoryID = c.ID
                WHERE wi.CustomerID = @CustomerID";

            List<SqlParameter> parameters = new List<SqlParameter> { new SqlParameter("@CustomerID", customerId) };

            DataTable result = await this.databaseService.ExecuteSelectAsync(selectQuery, parameters);
            List<WishlistItem> wishlistItems = new List<WishlistItem>();

            foreach (DataRow row in result.Rows)
            {
                Category category = new Category(
                    id: Convert.ToInt32(row["CategoryID"]),
                    name: Convert.ToString(row["CategoryName"]) ?? string.Empty);

                Product product = new Product(
                            id: Convert.ToInt32(row["ProductID"]),
                            name: row["Name"]?.ToString() ?? string.Empty,
                            price: Convert.ToDecimal(row["Price"]),
                            stock: Convert.ToInt32(row["Stock"]),
                            category: category,
                            size: row["Size"]?.ToString() ?? string.Empty,
                            color: row["Color"]?.ToString() ?? string.Empty,
                            description: row["Description"]?.ToString() ?? string.Empty,
                            photoURL: row["PhotoURL"]?.ToString());

                WishlistItem wishlistItem = new WishlistItem(
                    id: Convert.ToInt32(row["WishlistID"]),
                    product: product,
                    customerID: Convert.ToInt32(row["CustomerID"]));

                wishlistItems.Add(wishlistItem);
            }

            return wishlistItems;
        }

        /// <summary>
        /// Retrieves a specific wishlist item by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the wishlist item to retrieve.</param>
        /// <returns>
        /// A <see cref="WishlistItem"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<WishlistItem?> GetByIdAsync(int id)
        {
            string selectQuery = @"
                SELECT wi.ID AS WishlistID, wi.CustomerID, 
                       p.ID AS ProductID, p.Name, p.Price, p.Stock, p.Size, p.Color, p.Description, p.PhotoURL,
                       c.ID AS CategoryID, c.Name AS CategoryName
                FROM WishlistItem wi
                INNER JOIN Product p ON wi.ProductID = p.ID
                INNER JOIN Category c ON p.CategoryID = c.ID
                WHERE wi.ID = @ID";

            List<SqlParameter> parameters = new List<SqlParameter> { new SqlParameter("@ID", id) };
            DataTable result = await this.databaseService.ExecuteSelectAsync(selectQuery, parameters);

            if (result.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = result.Rows[0];

            Category category = new Category(
                id: Convert.ToInt32(row["CategoryID"]),
                name: Convert.ToString(row["CategoryName"]) ?? string.Empty);

            Product product = new Product(
                            id: Convert.ToInt32(row["ProductID"]),
                            name: row["Name"]?.ToString() ?? string.Empty,
                            price: Convert.ToDecimal(row["Price"]),
                            stock: Convert.ToInt32(row["Stock"]),
                            category: category,
                            size: row["Size"]?.ToString() ?? string.Empty,
                            color: row["Color"]?.ToString() ?? string.Empty,
                            description: row["Description"]?.ToString() ?? string.Empty,
                            photoURL: row["PhotoURL"]?.ToString());

            return new WishlistItem(
                id: Convert.ToInt32(row["WishlistID"]),
                product: product,
                customerID: Convert.ToInt32(row["CustomerID"]));
        }

        /// <summary>
        /// Inserts a new wishlist item into the database for the current customer.
        /// </summary>
        /// <param name="entity">The <see cref="WishlistItem"/> to insert.</param>
        /// <returns>
        /// The same <see cref="WishlistItem"/> entity with the newly generated ID assigned.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the current user ID is not available in the session.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if the insertion fails or the new ID is invalid.
        /// </exception>
        public async Task<WishlistItem> CreateAsync(WishlistItem entity)
        {
            int customerId = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            string insertQuery = @"
                        INSERT INTO WishlistItem (ProductID, CustomerID)
                        VALUES (@ProductID, @CustomerID);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProductID", entity.Product.ID),
                new SqlParameter("@CustomerID", customerId),
            };

            int newId = await this.databaseService.ExecuteScalarAsync<int>(insertQuery, parameters);

            if (newId < 0)
            {
                throw new Exception($"Error inserting wishlist item for product ID {entity.Product.ID}");
            }

            entity.ID = newId;
            return entity;
        }

        /// <summary>
        /// Placeholder for updating a wishlist item.
        /// Currently not implemented, as updates are not required.
        /// </summary>
        /// <param name="entity">The wishlist item to "update".</param>
        /// <returns>
        /// The same <see cref="WishlistItem"/> entity passed in.
        /// </returns>
        public async Task<WishlistItem> UpdateAsync(WishlistItem entity)
        {
            // Wishlist item update not required.
            return await Task.FromResult(entity);
        }

        /// <summary>
        /// Deletes a wishlist item from the database based on its ID.
        /// </summary>
        /// <param name="id">The ID of the wishlist item to delete.</param>
        /// <returns>
        /// <c>true</c> if the deletion was successful; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> DeleteAsync(int id)
        {
            string deleteQuery = "DELETE FROM WishlistItem WHERE ID = @ID";
            List<SqlParameter> parameters = new List<SqlParameter> { new SqlParameter("@ID", id) };

            int result = await this.databaseService.ExecuteQueryAsync(deleteQuery, parameters);
            return result > 0;
        }
    }
}
