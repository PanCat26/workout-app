// <copyright file="CartItemRepository.cs" company="PlaceholderCompany">
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
    using WorkoutApp.Models;

    /// <summary>
    /// Provides CRUD operations for cart items in the database.
    /// </summary>
    public class CartItemRepository : IRepository<CartItem>
    {
        private readonly DbService databaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItemRepository"/> class.
        /// </summary>
        /// <param name="dBService">The database service used for database operations.</param>
        public CartItemRepository(DbService dBService)
        {
            this.databaseService = dBService;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            int cartId = await this.GetActiveCartId();
            List<CartItem> cartItems = new List<CartItem>();

            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT * FROM CartItem WHERE IsActive = 1 AND CartId = @CartId",
                    new List<SqlParameter> { new SqlParameter("@CartId", cartId) });

                foreach (DataRow row in selectQueryResult.Rows)
                {
                    CartItem cartItem = new CartItem
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        CartId = Convert.ToInt32(row["CartId"]),
                        ProductId = Convert.ToInt32(row["ProductID"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                    };
                    cartItems.Add(cartItem);
                }

                return cartItems;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving cart items: {exception.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task<CartItem> GetByIdAsync(long id)
        {
            int cartId = await this.GetActiveCartId();

            try
            {
                var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                    "SELECT * FROM CartItem WHERE IsActive = 1 AND CartId = @CartId AND ID = @Id",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@CartId", cartId),
                        new SqlParameter("@Id", id),
                    });

                if (selectQueryResult.Rows.Count == 0)
                {
                    throw new Exception("Cart item not found.");
                }

                DataRow row = selectQueryResult.Rows[0];
                CartItem cartItem = new CartItem
                {
                    Id = Convert.ToInt32(row["ID"]),
                    CartId = Convert.ToInt32(row["CartId"]),
                    ProductId = Convert.ToInt32(row["ProductID"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                };

                return cartItem;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error retrieving cart item: {exception.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task<CartItem> CreateAsync(CartItem newCartItem)
        {
            int cartId = await this.GetActiveCartId();

            int insertQueryResult = await this.databaseService.ExecuteQueryAsync(
                "INSERT INTO CartItem (Id, CartId, ProductId, Quantity, IsActive) VALUES (@Id, @CartId, @ProductId, @Quantity, @IsActive)",
                new List<SqlParameter>
                {
                    new SqlParameter("@Id", newCartItem.Id),
                    new SqlParameter("@CartId", cartId),
                    new SqlParameter("@ProductId", newCartItem.ProductId),
                    new SqlParameter("@Quantity", newCartItem.Quantity),
                    new SqlParameter("@IsActive", true),
                });

            if (insertQueryResult < 0)
            {
               throw new Exception($"Error inserting cart item with id: {newCartItem.Id}");
            }

            return newCartItem;
        }

        /// <inheritdoc/>
        public async Task<CartItem> UpdateAsync(CartItem cartItem)
        {
            int updateQueryResult = await this.databaseService.ExecuteQueryAsync(
                "UPDATE CartItem SET Quantity = @Quantity WHERE Id = @Id",
                new List<SqlParameter>
                {
                    new SqlParameter("@Id", cartItem.Id),
                    new SqlParameter("@Quantity", cartItem.Quantity),
                });

            if (updateQueryResult < 0)
            {
                throw new Exception($"Error updating cart item with id: {cartItem.Id}");
            }

            return cartItem;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(long itemId)
        {
            int deleteQueryResult = await this.databaseService.ExecuteQueryAsync(
                "UPDATE CartItem SET IsActive = 0 WHERE ID = @Id",
                new List<SqlParameter> { new SqlParameter("@Id", itemId) });

            if (deleteQueryResult < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resets the cart by deleting all active items and creating a new cart.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> ResetCart()
        {
            IEnumerable<CartItem> cartItems = this.GetAllAsync().Result;

            foreach (CartItem cartItem in cartItems)
            {
                this.DeleteAsync(cartItem.Id).Wait();
            }

            int newId = await this.GetActiveCartId();

            var insertQueryResult = await this.databaseService.ExecuteQueryAsync(
                "INSERT INTO Cart (Id, CustomerID, CreatedAt, IsActive) VALUES (@Id, 1, GETDATE(), 1)",
                new List<SqlParameter> { new SqlParameter("@Id", newId + 1) });

            if (insertQueryResult < 0)
            {
                throw new Exception($"Error inserting new cart with id: {newId + 1}");
            }

            var updateQueryResult = await this.databaseService.ExecuteQueryAsync(
                "UPDATE Cart SET IsActive = 0 WHERE Id = @Id",
                new List<SqlParameter> { new SqlParameter("@Id", newId) });

            if (updateQueryResult < 0)
            {
                throw new Exception($"Error updating cart with id: {newId}");
            }

            return true;
        }

        /// <summary>
        /// Retrieves the ID of the active cart.
        /// </summary>
        /// <returns>The ID of the active cart.</returns>
        private async Task<int> GetActiveCartId()
        {
            var selectQueryResult = await this.databaseService.ExecuteSelectAsync(
                "SELECT ISNULL(MAX(ID), 0) FROM Cart",
                new List<SqlParameter>());

            if (selectQueryResult.Rows.Count == 0)
            {
                throw new Exception("No active cart found.");
            }

            int cartId = Convert.ToInt32(selectQueryResult.Rows[0][0]);

            return cartId;
        }
    }
}
