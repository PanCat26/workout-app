// <copyright file="CartItemRepository.cs" company="WorkoutApp">
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
            int customerID = (int)this.sessionManager.CurrentUserId;
            List<CartItem> cartItems = new List<CartItem>();

            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT * FROM CartItem WHERE CustomerID = @CustomerID",
                    new List<SqlParameter> { new SqlParameter("@CustomerID", customerID) });

                foreach (DataRow row in selectQueryResult.Rows)
                {
                    CartItem cartItem = new CartItem(
                        productID: Convert.ToInt32(row["ProductID"]),
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
        /// <returns>The cart item with the specified ID.</returns>
        public async Task<CartItem> GetByIdAsync(int productID)
        {
            int customerID = (int)this.sessionManager.CurrentUserId;
            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT * FROM CartItem WHERE CustomerID = @CustomerID AND ProductID = @ProductId",
                    new List<SqlParameter>
                    {
                            new SqlParameter("@CartId", customerID),
                            new SqlParameter("@Id", productID),
                    });

                if (selectQueryResult.Rows.Count == 0)
                {
                    throw new Exception("Cart item not found.");
                }

                DataRow row = selectQueryResult.Rows[0];
                CartItem cartItem = new CartItem(
                        productID: Convert.ToInt32(row["ProductID"]),
                        customerID: Convert.ToInt32(row["CustomerID"]),
                        quantity: Convert.ToInt32(row["Quantity"]));

                return cartItem;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving cart item: {exception.Message}");
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
            int customerID = (int)this.sessionManager.CurrentUserId;

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

            return;
        }

        /// <summary>
        /// Updates an existing cart item asynchronously.
        /// </summary>
        /// <param name="cartItem">The cart item to update.</param>
        /// <returns>The updated cart item.</returns>
        public async Task<CartItem> UpdateAsync(CartItem cartItem)
        {
            bool isActive = cartItem.Quantity > 0;
            int updateQueryResult = await this.databaseService.ExecuteQueryAsync(
                "UPDATE CartItem SET Quantity = @Quantity WHERE ProductID = @ProductID AND CustomerID = @CustomerID",
                new List<SqlParameter>
                {
                        new SqlParameter("@ProductID", cartItem.ProductID),
                        new SqlParameter("@CustomerID", cartItem.CustomerID),
                        new SqlParameter("@Quantity", cartItem.Quantity),
                });

            if (updateQueryResult < 0)
            {
                throw new Exception($"Error updating cart item with id: {cartItem.ID}");
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
            int deleteQueryResult = await this.databaseService.ExecuteQueryAsync(
                "DELETE CartItem WHERE ProductID = @ProductID",
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
            IEnumerable<CartItem> cartItems = await this.GetAllAsync();

            int customerID = (int)this.sessionManager.CurrentUserId;

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
            int customerID = (int)this.sessionManager.CurrentUserId;

            int insertQueryResult = await this.databaseService.ExecuteQueryAsync(
                "INSERT INTO CartItem (ProductId, CustomerID, Quantity) VALUES (@CustomerID, @ProductId, @Quantity)",
                new List<SqlParameter>
                {
                        new SqlParameter("@ProductId", entity.ProductID),
                        new SqlParameter("@CustomerID", entity.CustomerID),
                        new SqlParameter("@Quantity", entity.Quantity),
                });

            if (insertQueryResult < 0)
            {
                throw new Exception($"Error inserting cart item with product id: {entity.ProductID}");
            }

            return entity;
        }
    }
}
