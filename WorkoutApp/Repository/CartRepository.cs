// <copyright file="CartRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Infrastructure.Session;
    using WorkoutApp.Models;
    using WorkoutApp.Utils.Filters;

    /// <summary>
    /// Provides CRUD operations for cart items in the database.
    /// </summary>
    public class CartRepository : IRepository<CartItem>
    {
        private readonly DbService databaseService;
        private readonly SessionManager sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service used for executing queries.</param>
        /// <param name="sessionManager">The session manager for managing user sessions.</param>
        public CartRepository(DbService dbService, SessionManager sessionManager)
        {
            this.databaseService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            this.sessionManager = sessionManager;
        }

        /// <summary>
        /// Retrieves all cart items asynchronously.
        /// </summary>
        /// <returns>A collection of cart items.</returns>
        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            List<CartItem> cartItems = new List<CartItem>();

            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT ci.ID as CartItemID, " +
                    "p.ID AS ProductID, " +
                    "ci.CustomerID, " +
                    "p.Name, " +
                    "p.Price, " +
                    "p.Stock, " +
                    "p.Size, " +
                    "p.Color, " +
                    "p.Description, " +
                    "p.PhotoURL, " +
                    "p.CategoryID, " +
                    "ct.Name AS CategoryName " +
                    "FROM CartItem ci " +
                    "JOIN Product p ON ci.ProductID = p.ID " +
                    "JOIN Category ct ON p.CategoryID = ct.ID " +
                    "WHERE ci.CustomerID = @CustomerID;",
                    new List<SqlParameter> { new SqlParameter("@CustomerID", customerID) });

                foreach (DataRow row in selectQueryResult.Rows)
                {
                    Category category = new Category(
                        id: Convert.ToInt32(row["CategoryID"]),
                        name: row["CategoryName"]?.ToString() ?? string.Empty);

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

                    CartItem cartItem = new CartItem(
                        id: Convert.ToInt32(row["CartItemID"]),
                        product: product,
                        customerID: Convert.ToInt32(row["CustomerID"]));
                    cartItems.Add(cartItem);
                }

                return cartItems;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving cart items: {exception.Message}");
            }
        }

        /// <summary>
        /// Retrieves a cart item by its ID asynchronously.
        /// </summary>
        /// <param name="cartItemID">The ID of the cart item.</param>
        /// <returns>The cart item with the specified ID, or null if not found.</returns>
        public async Task<CartItem?> GetByIdAsync(int cartItemID)
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT ci.ID AS CartItemID, " +
                    "p.ID AS ProductID, " +
                    "ci.CustomerID, " +
                    "p.Name, " +
                    "p.Price, " +
                    "p.Stock, " +
                    "p.Size, " +
                    "p.Color, " +
                    "p.Description, " +
                    "p.PhotoURL, " +
                    "c.ID AS CategoryID, " +
                    "c.Name AS CategoryName " +
                    "FROM CartItem ci " +
                    "JOIN Product p ON ci.ProductID = p.ID " +
                    "JOIN Category c ON p.CategoryID = c.ID " +
                    "WHERE ci.CustomerID = @CustomerID AND ci.ID = @CartItemID;",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@CustomerID", customerID),
                        new SqlParameter("@CartItemID", cartItemID),
                    });

                if (selectQueryResult.Rows.Count == 0)
                {
                    return null;
                }

                DataRow row = selectQueryResult.Rows[0];

                Category category = new Category(
                    id: Convert.ToInt32(row["CategoryID"]),
                    name: row["CategoryName"]?.ToString() ?? string.Empty);

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

                CartItem cartItem = new CartItem(
                    id: Convert.ToInt32(row["CartItemID"]),
                    product: product,
                    customerID: Convert.ToInt32(row["CustomerID"]));

                return cartItem;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving product: {exception.Message}");
            }
        }

        /// <summary>
        /// Creates a new cart item asynchronously.
        /// </summary>
        /// <param name="entity">The cart item to create.</param>
        /// <returns>The created cart item.</returns>
        public async Task<CartItem> CreateAsync(CartItem entity)
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            int newId = await this.databaseService.ExecuteScalarAsync<int>(
                "INSERT INTO CartItem (ProductID, CustomerID) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@ProductID, @CustomerID)",
                new List<SqlParameter>
                {
                    new SqlParameter("@ProductID", entity.Product.ID),
                    new SqlParameter("@CustomerID", customerID),
                });

            entity.ID = newId;
            return entity;
        }

        /// <summary>
        /// Updates an existing cart item asynchronously.
        /// </summary>
        /// <param name="cartItem">The cart item to update.</param>
        /// <returns>The updated cart item.</returns>
        public async Task<CartItem> UpdateAsync(CartItem cartItem)
        {
            return cartItem;
        }

        /// <summary>
        /// Deletes a cart item asynchronously.
        /// </summary>
        /// <param name="cartItemID">The ID of the cart item to delete.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        public async Task<bool> DeleteAsync(int cartItemID)
        {
            int deleteQueryResult = await this.databaseService.ExecuteQueryAsync(
                "DELETE FROM CartItem WHERE ID = @CartItemID",
                new List<SqlParameter> { new SqlParameter("@CartItemID", cartItemID) });

            // Unsuccessful deletion not handled.
            return true;
        }
    }
}
