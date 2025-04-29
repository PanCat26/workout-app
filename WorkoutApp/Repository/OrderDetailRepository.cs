// <copyright file="OrderDetailRepository.cs" company="PlaceholderCompany">
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
    /// Provides CRUD operations for order details in the database.
    /// </summary>
    public class OrderDetailRepository : IRepository<OrderDetail>
    {
        private readonly DbService dbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service to use for operations.</param>
        public OrderDetailRepository(DbService dbService)
        {
            this.dbService = dbService;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            const string query = "SELECT * FROM OrderDetail";
            var parameters = new List<SqlParameter>();

            DataTable result = await this.dbService.ExecuteSelectAsync(query, parameters);
            var orderDetails = new List<OrderDetail>();

            foreach (DataRow row in result.Rows)
            {
                orderDetails.Add(new OrderDetail(
                    iD: Convert.ToInt32(row["ID"]),
                    orderID: Convert.ToInt32(row["OrderID"]),
                    productID: Convert.ToInt32(row["ProductID"]),
                    quantity: Convert.ToInt32(row["Quantity"]),
                    price: Convert.ToDouble(row["Price"]),
                    isActive: Convert.ToBoolean(row["IsActive"])));
            }

            return orderDetails;
        }

        /// <inheritdoc/>
        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM OrderDetail WHERE ID = @ID";
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
            return new OrderDetail(
                iD: Convert.ToInt32(row["ID"]),
                orderID: Convert.ToInt32(row["OrderID"]),
                productID: Convert.ToInt32(row["ProductID"]),
                quantity: Convert.ToInt32(row["Quantity"]),
                price: Convert.ToDouble(row["Price"]),
                isActive: Convert.ToBoolean(row["IsActive"]));
        }

        /// <inheritdoc/>
        public async Task<OrderDetail> CreateAsync(OrderDetail entity)
        {
            const string query = @"
                INSERT INTO OrderDetail (OrderID, ProductID, Quantity, Price, IsActive)
                VALUES (@OrderID, @ProductID, @Quantity, @Price, @IsActive);
                SELECT SCOPE_IDENTITY();";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@OrderID", entity.OrderID),
                new SqlParameter("@ProductID", entity.ProductID),
                new SqlParameter("@Quantity", entity.Quantity),
                new SqlParameter("@Price", entity.Price),
                new SqlParameter("@IsActive", entity.IsActive),
            };

            await this.dbService.ExecuteQueryAsync(query, parameters);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<OrderDetail> UpdateAsync(OrderDetail entity)
        {
            const string query = @"
                UPDATE OrderDetail 
                SET OrderID = @OrderID,
                    ProductID = @ProductID,
                    Quantity = @Quantity,
                    Price = @Price,
                    IsActive = @IsActive
                WHERE ID = @ID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", entity.ID),
                new SqlParameter("@OrderID", entity.OrderID),
                new SqlParameter("@ProductID", entity.ProductID),
                new SqlParameter("@Quantity", entity.Quantity),
                new SqlParameter("@Price", entity.Price),
                new SqlParameter("@IsActive", entity.IsActive),
            };

            await this.dbService.ExecuteQueryAsync(query, parameters);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "UPDATE OrderDetail SET IsActive = 0 WHERE ID = @ID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id),
            };

            int rowsAffected = await this.dbService.ExecuteQueryAsync(query, parameters);
            return rowsAffected > 0;
        }
    }
}
