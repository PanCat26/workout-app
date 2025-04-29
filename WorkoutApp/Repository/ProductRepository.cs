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
    /// Implements the <see cref="IRepository{IProduct}"/> interface.
    /// </summary>
    public class ProductRepository : IRepository<IProduct>
    {
        private readonly DbService dbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service.</param>
        public ProductRepository(DbService dbService)
        {
            this.dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IProduct>> GetAllAsync()
        {
            var query = "SELECT * FROM Product WHERE IsActive = 1";
            DataTable table = await this.dbService.ExecuteSelectAsync(query, []);

            var products = new List<IProduct>();
            foreach (DataRow row in table.Rows)
            {
                products.Add(this.MapRowToProduct(row));
            }

            return products;
        }

        /// <inheritdoc/>
        public async Task<IProduct> GetByIdAsync(long id)
        {
            var query = "SELECT * FROM Product WHERE IsActive = 1 AND ID = @ID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };

            DataTable table = await this.dbService.ExecuteSelectAsync(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            return this.MapRowToProduct(table.Rows[0]);
        }

        /// <inheritdoc/>
        public async Task<IProduct> CreateAsync(IProduct entity)
        {
            var query = @"
                INSERT INTO Product (Name, Price, Stock, CategoryID, Attributes, Size, Description, FileUrl, IsActive)
                OUTPUT INSERTED.ID
                VALUES (@Name, @Price, @Stock, @CategoryID, @Attributes, @Size, @Description, @FileUrl, @IsActive);";

            var parameters = this.FillInsertCommandParameters(entity);

            DataTable table = await this.dbService.ExecuteSelectAsync(query, parameters);

            entity.ID = Convert.ToInt32(table.Rows[0][0]); // Set the newly inserted ID
            return entity;
        }

        /// <inheritdoc/>
        public async Task<IProduct> UpdateAsync(IProduct entity)
        {
            var query = @"
                UPDATE Product
                SET Name = @Name,
                    Price = @Price,
                    Stock = @Stock,
                    CategoryID = @CategoryID,
                    Attributes = @Attributes,
                    Size = @Size,
                    Description = @Description,
                    FileUrl = @FileUrl
                WHERE ID = @ID;";

            var parameters = this.FillUpdateCommandParameters(entity);

            await this.dbService.ExecuteQueryAsync(query, parameters);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(long id)
        {
            var query = "UPDATE Product SET IsActive = 0 WHERE ID = @ID";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id)
            };

            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);
            return affectedRows > 0;
        }

        private IProduct MapRowToProduct(DataRow row)
        {
            int category = Convert.ToInt32(row["CategoryID"]);

            if (category == 1)
            {
                return new ClothesProduct(
                    id: Convert.ToInt32(row["ID"]),
                    name: row["Name"].ToString(),
                    price: Convert.ToDouble(row["Price"]),
                    stock: Convert.ToInt32(row["Stock"]),
                    categoryId: category,
                    color: row["Attributes"]?.ToString() ?? string.Empty,
                    size: row["Size"]?.ToString() ?? string.Empty,
                    description: row["Description"].ToString(),
                    fileUrl: row["FileUrl"].ToString(),
                    isActive: true);
            }
            else if (category == 2)
            {
                return new FoodProduct(
                    id: Convert.ToInt32(row["ID"]),
                    name: row["Name"].ToString(),
                    price: Convert.ToDouble(row["Price"]),
                    stock: Convert.ToInt32(row["Stock"]),
                    categoryId: category,
                    size: row["Size"]?.ToString() ?? string.Empty,
                    description: row["Description"].ToString(),
                    fileUrl: row["FileUrl"].ToString(),
                    isActive: true);
            }
            else
            {
                return new AccessoryProduct(
                    id: Convert.ToInt32(row["ID"]),
                    name: row["Name"].ToString(),
                    price: Convert.ToDouble(row["Price"]),
                    stock: Convert.ToInt32(row["Stock"]),
                    categoryId: category,
                    description: row["Description"].ToString(),
                    fileUrl: row["FileUrl"].ToString(),
                    isActive: true);
            }
        }

        private List<SqlParameter> FillInsertCommandParameters(IProduct product)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@Stock", product.Stock),
                new SqlParameter("@CategoryID", product.CategoryID),
                new SqlParameter("@Description", product.Description),
                new SqlParameter("@FileUrl", product.FileUrl),
                new SqlParameter("@IsActive", true)
            };

            if (product.CategoryID == 1)
            {
                parameters.Add(new SqlParameter("@Attributes", ((ClothesProduct)product).Attributes ?? string.Empty));
                parameters.Add(new SqlParameter("@Size", ((ClothesProduct)product).Size ?? string.Empty));
            }
            else if (product.CategoryID == 2)
            {
                parameters.Add(new SqlParameter("@Attributes", string.Empty));
                parameters.Add(new SqlParameter("@Size", ((FoodProduct)product).Size ?? string.Empty));
            }
            else
            {
                parameters.Add(new SqlParameter("@Attributes", string.Empty));
                parameters.Add(new SqlParameter("@Size", string.Empty));
            }

            return parameters;
        }

        private List<SqlParameter> FillUpdateCommandParameters(IProduct product)
        {
            var parameters = FillInsertCommandParameters(product);
            parameters.Add(new SqlParameter("@ID", product.ID));
            return parameters;
        }
    }
}
