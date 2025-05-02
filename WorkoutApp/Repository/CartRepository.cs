// <copyright file="CartRepository.cs" company="PlaceholderCompany">
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
                    "SELECT p.ID AS ProductID, " +
                    "ci.CustomerID, " +
                    "ci.Quantity, " +
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
                        product: product,
                        customerID: Convert.ToInt32(row["CustomerID"]),
                        quantity: Convert.ToInt32(row["Quantity"]));
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
        /// <param name="productID">The ID of the cart item.</param>
        /// <returns>The cart item with the specified ID, or null if not found.</returns>
        public async Task<CartItem?> GetByIdAsync(int productID)
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT p.ID AS ProductID, " +
                    "ci.CustomerID, " +
                    "ci.Quantity, " +
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
                    "WHERE ci.CustomerID = @CustomerID AND p.ID = @ProductID;",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@CustomerID", customerID),
                        new SqlParameter("@ProductID", productID),
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
                    product: product,
                    customerID: Convert.ToInt32(row["CustomerID"]),
                    quantity: Convert.ToInt32(row["Quantity"]));

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
        /// <param name="productId">The product ID of the cart item.</param>
        /// <param name="quantity">The quantity of the cart item.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task CreateAsync(int productId, int quantity)
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            int insertQueryResult = await this.databaseService.ExecuteQueryAsync(
                "INSERT INTO CartItem (ProductID, CustomerID, Quantity) VALUES (@ProductID, @CustomerID, @Quantity)",
                new List<SqlParameter>
                {
                    new SqlParameter("@ProductID", productId),
                    new SqlParameter("@CustomerID", customerID),
                    new SqlParameter("@Quantity", quantity),
                });

            if (insertQueryResult < 0)
            {
                throw new Exception($"Error inserting cart item with product id: {productId}");
            }
        }

        /// <summary>
        /// Updates an existing cart item asynchronously.
        /// </summary>
        /// <param name="cartItem">The cart item to update.</param>
        /// <returns>The updated cart item.</returns>
        public async Task<CartItem> UpdateAsync(CartItem cartItem)
        {
            int updateQueryResult = await this.databaseService.ExecuteQueryAsync(
                "UPDATE CartItem SET Quantity = @Quantity WHERE ProductID = @ProductID AND CustomerID = @CustomerID",
                new List<SqlParameter>
                {
                    new SqlParameter("@ProductID", cartItem.Product.ID),
                    new SqlParameter("@CustomerID", cartItem.CustomerID),
                    new SqlParameter("@Quantity", cartItem.Quantity),
                });

            if (updateQueryResult < 0)
            {
                throw new Exception($"Error updating cart item with product id: {cartItem.Product.ID}");
            }

            return cartItem;
        }

        /// <summary>
        /// Deletes a cart item asynchronously.
        /// </summary>
        /// <param name="productID">The ID of the cart item to delete.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        public async Task<bool> DeleteAsync(int productID)
        {
            if (productID <= 0)
            {
                throw new ArgumentException("Invalid product ID.");
            }

            int deleteQueryResult = await this.databaseService.ExecuteQueryAsync(
                "DELETE FROM CartItem WHERE ProductID = @ProductID",
                new List<SqlParameter> { new SqlParameter("@ProductID", productID) });

            if (deleteQueryResult < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resets the cart by deleting all active items and creating a new cart asynchronously.
        /// </summary>
        /// <returns>A boolean indicating whether the reset was successful.</returns>
        public async Task<bool> ResetCart()
        {
            int customerID = this.sessionManager.CurrentUserId ?? throw new InvalidOperationException("Current user ID is null.");

            if (customerID <= 0)
            {
                throw new ArgumentException("Invalid customer ID.");
            }

            string deleteQuery = "DELETE FROM CartItem WHERE CustomerID = @CustomerID";
            int deleteQueryResult = await this.databaseService.ExecuteQueryAsync(
                deleteQuery,
                new List<SqlParameter> { new SqlParameter("@CustomerID", customerID) });

            if (deleteQueryResult < 0)
            {
                throw new Exception($"Error deleting cart items for customer id: {customerID}");
            }

            return true;
        }

        /// <summary>
        /// Creates a new cart item asynchronously.
        /// </summary>
        /// <param name="entity">The cart item to create.</param>
        /// <returns>The created cart item.</returns>
        public async Task<CartItem> CreateAsync(CartItem entity)
        {

            int insertQueryResult = await this.databaseService.ExecuteQueryAsync(
                "INSERT INTO CartItem (ProductID, CustomerID, Quantity) VALUES (@ProductID, @CustomerID, @Quantity)",
                new List<SqlParameter>
                {
                    new SqlParameter("@ProductID", entity.Product.ID),
                    new SqlParameter("@CustomerID", entity.CustomerID),
                    new SqlParameter("@Quantity", entity.Quantity),
                });

            if (insertQueryResult < 0)
            {
                throw new Exception($"Error inserting cart item with product id: {entity.Product.ID}");
            }

            return entity;
        }
    }
}
