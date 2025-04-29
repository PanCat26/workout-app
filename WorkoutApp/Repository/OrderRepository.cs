// <copyright file="OrderRepository.cs" company="PlaceholderCompany">
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
    /// Provides CRUD operations for orders in the database.
    /// </summary>
    public class OrderRepository : IRepository<Order>
    {
        private readonly DbService dbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service to use for operations.</param>
        public OrderRepository(DbService dbService)
        {
            this.dbService = dbService;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            const string query = "SELECT * FROM [Order]";
            var parameters = new List<SqlParameter>();

            DataTable result = await this.dbService.ExecuteSelectAsync(query, parameters);
            var orders = new List<Order>();

            foreach (DataRow row in result.Rows)
            {
                orders.Add(new Order(
                    iD: Convert.ToInt32(row["ID"]),
                    customerID: Convert.ToInt32(row["CustomerID"]),
                    orderDate: Convert.ToDateTime(row["OrderDate"]),
                    totalAmount: Convert.ToDouble(row["TotalAmount"]),
                    isActive: Convert.ToBoolean(row["IsActive"])));
            }

            return orders;
        }

        /// <inheritdoc/>
        public async Task<Order> GetByIdAsync(long id)
        {
            const string query = "SELECT * FROM [Order] WHERE ID = @ID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id),
            };

            DataTable result = await this.dbService.ExecuteSelectAsync(query, parameters);

            if (result.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = result.Rows[0];
            return new Order(
                iD: Convert.ToInt32(row["ID"]),
                customerID: Convert.ToInt32(row["CustomerID"]),
                orderDate: Convert.ToDateTime(row["OrderDate"]),
                totalAmount: Convert.ToDouble(row["TotalAmount"]),
                isActive: Convert.ToBoolean(row["IsActive"]));
        }

        /// <inheritdoc/>
        public async Task<Order> CreateAsync(Order entity)
        {
            const string query = @"
                INSERT INTO [Order] (CustomerID, OrderDate, TotalAmount, IsActive)
                VALUES (@CustomerID, @OrderDate, @TotalAmount, @IsActive);
                SELECT SCOPE_IDENTITY();";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerID", entity.CustomerID),
                new SqlParameter("@OrderDate", entity.OrderDate),
                new SqlParameter("@TotalAmount", entity.TotalAmount),
                new SqlParameter("@IsActive", entity.IsActive),
            };
            await this.dbService.ExecuteQueryAsync(query, parameters);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<Order> UpdateAsync(Order entity)
        {
            const string query = @" UPDATE [Order] 
                SET CustomerID = @CustomerID,
                    OrderDate = @OrderDate,
                    TotalAmount = @TotalAmount,
                    IsActive = @IsActive
                WHERE ID = @ID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", entity.ID),
                new SqlParameter("@CustomerID", entity.CustomerID),
                new SqlParameter("@OrderDate", entity.OrderDate),
                new SqlParameter("@TotalAmount", entity.TotalAmount),
                new SqlParameter("@IsActive", entity.IsActive),
            };
            await this.dbService.ExecuteQueryAsync(query, parameters);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(long id)
        {
            const string query = "UPDATE [Order] SET IsActive = 0 WHERE ID = @ID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id),
            };
            int rowsAffected = await this.dbService.ExecuteQueryAsync(query, parameters);
            return rowsAffected > 0;
        }
    }
}
