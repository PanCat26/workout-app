// <copyright file="OrderRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Infrastructure.Session;
    using WorkoutApp.Models;

    /// <summary>
    /// Provides CRUD operations for orders in the database.
    /// </summary>
    public class OrderRepository : IRepository<Order>
    {
        private readonly DbService dbService;
        private readonly SessionManager sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service to use for operations.</param>
        /// <param name="sessionManager">The session manager to use for user sessions.</param>
        public OrderRepository(DbService dbService, SessionManager sessionManager)
        {
            this.dbService = dbService;
            this.sessionManager = sessionManager;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return new List<Order>();
        }

        /// <inheritdoc/>
        public async Task<Order> GetByIdAsync(int id)
        {
            return new Order(id, new List<OrderItem>(), DateTime.Now);
        }

        /// <inheritdoc/>
        public async Task<Order> CreateAsync(Order entity)
        {
            int? customerId = this.sessionManager.CurrentUserId;

            const string insertOrderQuery = @"
                INSERT INTO [Order] (CustomerID, OrderDate)
                VALUES (@CustomerID, @OrderDate);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var orderParameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@OrderDate", entity.OrderDate),
            };

            // Get the generated Order ID
            int orderId = await this.dbService.ExecuteScalarAsync<int>(insertOrderQuery, orderParameters);

            foreach (var item in entity.OrderItems)
            {
                const string insertOrderItemQuery = @"
                    INSERT INTO OrderItem (OrderID, ProductID, Quantity)
                    VALUES (@OrderID, @ProductID, @Quantity);";

                var itemParams = new List<SqlParameter>
                {
                    new SqlParameter("@OrderID", orderId),
                    new SqlParameter("@ProductID", item.Product.ID),
                    new SqlParameter("@Quantity", item.Quantity),
                };

                await this.dbService.ExecuteQueryAsync(insertOrderItemQuery, itemParams);
            }

            entity.ID = orderId;
            return entity;
        }

        /// <inheritdoc/>
        public async Task<Order> UpdateAsync(Order entity)
        {
            return new Order(0, new List<OrderItem>(), DateTime.Now);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            return true;
        }
    }
}
