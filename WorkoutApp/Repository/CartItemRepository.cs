// <copyright file="CartItemRepository.cs" company="WorkoutApp">
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
    /// Provides CRUD operations for cart items in the database.
    /// </summary>
    public class CartItemRepository : IRepository<CartItem>
    {
        private readonly string connectionString = @"Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItemRepository"/> class.
        /// </summary>
        public CartItemRepository()
        {
            this.connection = new SqlConnection(this.connectionString);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            int cartId = this.GetActiveCartId();
            var cartItems = new List<CartItem>();

            this.connection.Open();
            using (SqlCommand selectCommand = new SqlCommand(
                "SELECT * FROM CartItem WHERE IsActive = 1 AND CartId = @CartId",
                this.connection))
            {
                selectCommand.Parameters.AddWithValue("@CartId", cartId);

                using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        CartItem cartItem = new CartItem
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ID")),
                            CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductID")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        };
                        cartItems.Add(cartItem);
                    }
                }
            }

            this.connection.Close();
            return cartItems;
        }

        /// <inheritdoc/>
        public async Task<CartItem> GetByIdAsync(int id)
        {
            int cartId = this.GetActiveCartId();

            this.connection.Open();
            using (SqlCommand selectCommand = new SqlCommand(
                "SELECT * FROM CartItem WHERE IsActive = 1 AND CartId = @CartId AND ID = @Id",
                this.connection))
            {
                selectCommand.Parameters.AddWithValue("@CartId", cartId);
                selectCommand.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        this.connection.Close();
                        return null;
                    }

                    CartItem cartItem = new CartItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID")),
                        CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                        ProductId = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    };

                    this.connection.Close();
                    return cartItem;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<CartItem> CreateAsync(CartItem entity)
        {
            int cartId = this.GetActiveCartId();

            this.connection.Open();

            SqlCommand getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(ID), 0) + 1 FROM CartItem", this.connection);
            int newId = (int)await getMaxIdCommand.ExecuteScalarAsync();

            using (SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO CartItem (Id, CartId, ProductId, Quantity, IsActive) VALUES (@Id, @CartId, @ProductId, @Quantity, @IsActive)",
                this.connection))
            {
                insertCommand.Parameters.AddWithValue("@Id", newId);
                insertCommand.Parameters.AddWithValue("@CartId", cartId);
                insertCommand.Parameters.AddWithValue("@ProductId", entity.ProductId);
                insertCommand.Parameters.AddWithValue("@Quantity", entity.Quantity);
                insertCommand.Parameters.AddWithValue("@IsActive", true);

                await insertCommand.ExecuteNonQueryAsync();
            }

            this.connection.Close();
            return entity;
        }

        /// <inheritdoc/>
        public async Task<CartItem> UpdateAsync(CartItem entity)
        {
            this.connection.Open();
            using (SqlCommand updateCommand = new SqlCommand(
                "UPDATE CartItem SET Quantity = @Quantity WHERE Id = @Id",
                this.connection))
            {
                updateCommand.Parameters.AddWithValue("@Id", entity.Id);
                updateCommand.Parameters.AddWithValue("@Quantity", entity.Quantity);

                await updateCommand.ExecuteNonQueryAsync();
            }

            this.connection.Close();
            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            this.connection.Open();
            using (SqlCommand updateCommand = new SqlCommand(
                "UPDATE CartItem SET IsActive = 0 WHERE ID = @Id",
                this.connection))
            {
                updateCommand.Parameters.AddWithValue("@Id", id);
                await updateCommand.ExecuteNonQueryAsync();
            }

            this.connection.Close();
            return true;
        }

        /// <summary>
        /// Resets the cart by deleting all active items and creating a new cart.
        /// </summary>
        public void ResetCart()
        {
            IEnumerable<CartItem> cartItems = this.GetAllAsync().Result;

            foreach (CartItem cartItem in cartItems)
            {
                this.DeleteAsync(cartItem.Id).Wait();
            }

            int newId = this.GetActiveCartId();

            this.connection.Open();

            using (SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO Cart (Id, CustomerID, CreatedAt, IsActive) VALUES (@Id, 1, GETDATE(), 1)",
                this.connection))
            {
                insertCommand.Parameters.AddWithValue("@Id", newId + 1);
                insertCommand.ExecuteNonQuery();
            }

            using (SqlCommand updateCommand = new SqlCommand(
                "UPDATE Cart SET IsActive = 0 WHERE Id = @Id",
                this.connection))
            {
                updateCommand.Parameters.AddWithValue("@Id", newId);
                updateCommand.ExecuteNonQuery();
            }

            this.connection.Close();
        }

        /// <summary>
        /// Retrieves the ID of the active cart.
        /// </summary>
        /// <returns>The ID of the active cart.</returns>
        private int GetActiveCartId()
        {
            if (this.connection.State == System.Data.ConnectionState.Closed)
            {
                this.connection.Open();
            }

            using (SqlCommand getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(ID), 0) FROM Cart", this.connection))
            {
                int newId = (int)getMaxIdCommand.ExecuteScalar();
                this.connection.Close();
                return newId;
            }
        }
    }
}
