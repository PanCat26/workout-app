// <copyright file="ProductRepository.cs" company="PlaceholderCompany">
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
    using WorkoutApp.Models;
    using WorkoutApp.Utils.Filters; // Using the Filters namespace

    /// <summary>
    /// Represents a repository for managing products in the database.
    /// Implements the <see cref="IRepository{Product}"/> interface.
    /// </summary>
    public class ProductRepository : IRepository<Product>
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
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            string query = @"
                SELECT
                    Product.ID AS ProductID,
                    Product.Name AS ProductName,
                    Product.Price,
                    Product.Stock,
                    Product.Size,
                    Product.Color,
                    Product.Description,
                    Product.PhotoURL,
                    Category.ID AS CategoryID,
                    Category.Name AS CategoryName
                FROM Product
                JOIN Category
                    ON Product.CategoryID = Category.ID;
            ";
            DataTable result = await this.dbService.ExecuteSelectAsync(query, new List<SqlParameter> { });

            List<Product> products = new List<Product> { };
            foreach (DataRow row in result.Rows)
            {
                products.Add(MapRowToProduct(row));
            }

            return products;
        }

        /// <inheritdoc/>
        public async Task<Product?> GetByIdAsync(int id)
        {
            string query = @"
                SELECT
                    Product.ID AS ProductID,
                    Product.Name AS ProductName,
                    Product.Price,
                    Product.Stock,
                    Product.Size,
                    Product.Color,
                    Product.Description,
                    Product.PhotoURL,
                    Category.ID AS CategoryID,
                    Category.Name AS CategoryName
                FROM Product
                JOIN Category
                    ON Product.CategoryID = Category.ID
                WHERE Product.ID = @ProductID;
            ";

            List<SqlParameter> parameters =
            [
                new ("@ProductID", id),
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
                VALUES (@Name, @Price, @Stock, @CategoryID, @Size, @Color, @Description, @PhotoURL);";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", entity.Name),
                new SqlParameter("@Price", entity.Price),
                new SqlParameter("@Stock", entity.Stock),
                new SqlParameter("@CategoryID", entity.Category.ID),
                new SqlParameter("@Size", entity.Size),
                new SqlParameter("@Color", entity.Color),
                new SqlParameter("@Description", entity.Description),
                new SqlParameter("@PhotoURL", entity.PhotoURL),
            };

            int id = await this.dbService.ExecuteScalarAsync<int>(query, parameters);

            entity.ID = id; // Set the newly inserted ID
            return entity;
        }

        /// <inheritdoc/>
        public async Task<Product> UpdateAsync(Product entity)
        {
            // Check if the product id is given
            if (entity.ID == null)
            {
                throw new ArgumentException("Product ID must be provided for update.");
            }

            // Check if the product exists
            _ = await this.GetByIdAsync((int)entity.ID) ?? throw new InvalidOperationException($"Product with ID {entity.ID} does not exist.");

            string query = @"
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

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", entity.Name),
                new SqlParameter("@Price", entity.Price),
                new SqlParameter("@Stock", entity.Stock),
                new SqlParameter("@CategoryID", entity.Category.ID),
                new SqlParameter("@Size", entity.Size),
                new SqlParameter("@Color", entity.Color),
                new SqlParameter("@Description", entity.Description),
                new SqlParameter("@PhotoURL", entity.PhotoURL),
            };

            await this.dbService.ExecuteQueryAsync(query, parameters);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            string query = "DELETE FROM Product WHERE ID = @ID;";
            List<SqlParameter> parameters =
            [
                new SqlParameter("@ID", id)
            ];

            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);
            return affectedRows > 0;
        }

        /// <inheritdoc/>
        // Implementing the GetAllFilteredAsync method from IRepository
        public async Task<IEnumerable<Product>> GetAllFilteredAsync(IFilter filter)
        {
            List<SqlParameter> parameters = new List<SqlParameter> { };
            ProductFilter productFilter = (ProductFilter)filter;

            // Add all filter parameters (even if null)
            parameters.Add(new SqlParameter("@CategoryID", (object?)productFilter.CategoryId ?? DBNull.Value));
            parameters.Add(new SqlParameter("@ExcludeProductID", (object?)productFilter.ExcludeProductId ?? DBNull.Value));
            parameters.Add(new SqlParameter("@Color", (object?)productFilter.Color ?? DBNull.Value));
            parameters.Add(new SqlParameter("@Size", (object?)productFilter.Size ?? DBNull.Value));
            parameters.Add(new SqlParameter("@SearchTerm", (object?)productFilter.SearchTerm ?? DBNull.Value));

            // Use TOP clause safely via string interpolation (not parameterizable)
            string topClause = string.Empty;
            if (productFilter.Count.HasValue && productFilter.Count > 0)
            {
                topClause = $"TOP ({productFilter.Count.Value})";
            }

            string whereClause = @"
                WHERE
                    (@CategoryID IS NULL OR Product.CategoryID = @CategoryID)
                AND (@ExcludeProductID IS NULL OR Product.ID != @ExcludeProductID)
                AND (@Color IS NULL OR Product.Color = @Color)
                AND (@Size IS NULL OR Product.Size = @Size)
                AND (
                    @SearchTerm IS NULL OR
                    Product.Name LIKE '%' + @SearchTerm + '%' OR
                    Product.Description LIKE '%' + @SearchTerm + '%'
                )";

            string query = $@"
                SELECT {topClause}
                    Product.ID AS ProductID,
                    Product.Name AS ProductName,
                    Product.Price,
                    Product.Stock,
                    Product.Size,
                    Product.Color,
                    Product.Description,
                    Product.PhotoURL,
                    Category.ID AS CategoryID,
                    Category.Name AS CategoryName
                FROM Product
                JOIN Category
                    ON Product.CategoryID = Category.ID
                {whereClause}
                ORDER BY Product.ID;";

            DataTable result = await this.dbService.ExecuteSelectAsync(query, parameters);

            List<Product> products = new List<Product> { };
            foreach (DataRow row in result.Rows)
            {
                products.Add(MapRowToProduct(row));
            }

            return products;
        }

        private static Product MapRowToProduct(DataRow row)
        {
            return new Product(
                     id: Convert.ToInt32(row["ProductID"]),
                     name: Convert.ToString(row["ProductName"]) !,
                     price: Convert.ToDecimal(row["Price"]),
                     stock: Convert.ToInt32(row["Stock"]),
                     category: new Category(
                         id: Convert.ToInt32(row["CategoryID"]),
                         name: Convert.ToString(row["CategoryName"]) !),
                     size: Convert.ToString(row["Size"]) !,
                     color: Convert.ToString(row["Color"]) !,
                     description: Convert.ToString(row["Description"]) !,
                     photoURL: Convert.ToString(row["PhotoURL"]));
        }
    }
}
