// <copyright file="ProductRepository.cs" company="WorkoutApp">
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
    using WorkoutApp.Models;

    /// <summary>
    /// Represents a repository for managing products in the database.
    /// Implements the <see cref="IRepository{Product}"/> interface.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ProductRepository"/> class.
    /// </remarks>
    /// <param name="dbService">The database service.</param>
    public class ProductRepository(DbService dbService) : IRepository<Product>
    {
        private readonly DbService dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));

        /// <inheritdoc/>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            string query = "SELECT Product.*, Category.Name FROM Product JOIN Category ON Product.CategoryID=Category.ID;";
            DataTable result = await this.dbService.ExecuteSelectAsync(query, []);

            List<Product> products = [];
            foreach (DataRow row in result.Rows)
            {
                products.Add(MapRowToProduct(row));
            }

            return products;
        }

        /// <inheritdoc/>
        public async Task<Product?> GetByIdAsync(int id)
        {
            string query = "SELECT Product.*, Category.Name FROM Product JOIN Category ON Product.CategoryID=Category.ID;";

            List<SqlParameter> parameters =
            [
                new ("@ID", id),
            ];

            DataTable table = await this.dbService.ExecuteSelectAsync(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            return MapRowToProduct(table.Rows[0]);
        }

        /// <inheritdoc/>
        public async Task<Product> CreateAsync(Product entity)
        {
            string query = @"
                INSERT INTO Product (Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL)
                OUTPUT INSERTED.ID
                VALUES (Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL);";

            List<SqlParameter> parameters = [
                new ("@Name", entity.Name),
                new ("@Price", entity.Price),
                new ("@Stock", entity.Stock),
                new ("@CategoryID", entity.Category.ID),
                new ("@Size", entity.Size),
                new ("@Color", entity.Color),
                new ("@Description", entity.Description),
                new ("@PhotoURL", entity.PhotoURL),
            ];

            int id = await this.dbService.ExecuteScalarAsync<int>(query, parameters);

            entity.ID = id; // Set the newly inserted ID
            return entity;
        }

        /// <inheritdoc/>
        public async Task<Product> UpdateAsync(Product entity)
        {
            var query = @"
                UPDATE Product
                SET Name = @Name,
                    Price = @Price,
                    Stock = @Stock,
                    CategoryID = @CategoryID,
                    Size = @Size,
                    Color = @Color,
                    Description = @Description,
                    PhotoURL = @PhotoURL
                WHERE ID = @ID;";

            List<SqlParameter> parameters = [
                new ("@ID", entity.ID),
                new ("@Name", entity.Name),
                new ("@Price", entity.Price),
                new ("@Stock", entity.Stock),
                new ("@CategoryID", entity.Category.ID),
                new ("@Size", entity.Size),
                new ("@Color", entity.Color),
                new ("@Description", entity.Description),
                new ("@PhotoURL", entity.PhotoURL),
            ];

            await this.dbService.ExecuteQueryAsync(query, parameters);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            string query = "UPDATE Product SET IsActive = 0 WHERE ID = @ID";

            List<SqlParameter> parameters =
            [
                new SqlParameter("@ID", id)
            ];

            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);
            return affectedRows > 0;
        }

        private static Product MapRowToProduct(DataRow row)
        {
            return new Product(
                    id: Convert.ToInt32(row["Product.ID"]),
                    name: Convert.ToString(row["Product.Name"]) !,
                    price: Convert.ToDecimal(row["Product.Price"]),
                    stock: Convert.ToInt32(row["Product.Stock"]),
                    category: new Category(id: Convert.ToInt32(row["Category.ID"]), name: Convert.ToString(row["Category.Name"]) !),
                    size: Convert.ToString(row["Size"]) !,
                    color: Convert.ToString(row["Attributes"]) !,
                    description: Convert.ToString(row["Description"]) !,
                    photoURL: Convert.ToString(row["PhotoURL"]));
        }
    }
}
