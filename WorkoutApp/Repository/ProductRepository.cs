// <copyright file="ProductRepository.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
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
            // Check if the filter is of the expected type ProductFilter
            if (filter is not ProductFilter productFilter)
            {
                // Handle invalid filter type - log or throw, returning empty is safest default
                System.Diagnostics.Debug.WriteLine($"Warning: GetAllFilteredAsync received unexpected filter type: {filter?.GetType().Name}. Expected ProductFilter.");
                return Enumerable.Empty<Product>();
            }

            List<SqlParameter> parameters = [];
            string topClause = "";
            string whereClause = " WHERE 1=1"; // Dummy condition to easily append more WHERE clauses

            // Add filter conditions based on the ProductFilter properties
            if (productFilter.CategoryId.HasValue && productFilter.CategoryId > 0)
            {
                whereClause += " AND Product.CategoryID = @CategoryID";
                parameters.Add(new SqlParameter("@CategoryID", productFilter.CategoryId.Value));
            }

            if (productFilter.ExcludeProductId.HasValue && productFilter.ExcludeProductId > 0)
            {
                whereClause += " AND Product.ID != @ExcludeProductID";
                parameters.Add(new SqlParameter("@ExcludeProductID", productFilter.ExcludeProductId.Value));
            }

            // Add TOP clause if Count is specified
            if (productFilter.Count.HasValue && productFilter.Count > 0)
            {
                topClause = $"TOP ({productFilter.Count.Value})";
            }

            // Build the final query
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
                ORDER BY Product.ID; -- Ensure ORDER BY is present for deterministic TOP results
            ";

            DataTable result = await this.dbService.ExecuteSelectAsync(query, parameters);

            List<Product> products = [];
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
                     name: Convert.ToString(row["ProductName"])!,
                     price: Convert.ToDecimal(row["Price"]),
                     stock: Convert.ToInt32(row["Stock"]),
                     category: new Category(
                         id: Convert.ToInt32(row["CategoryID"]),
                         name: Convert.ToString(row["CategoryName"])!),
                     size: Convert.ToString(row["Size"])!,
                     color: Convert.ToString(row["Color"])!,
                     description: Convert.ToString(row["Description"])!,
                     photoURL: Convert.ToString(row["PhotoURL"]));
        }
    }
}
