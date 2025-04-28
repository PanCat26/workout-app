// <copyright file="ProductRepository.cs" company="WorkoutApp">
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
    /// Represents a repository for managing products in the database.
    /// Implements the <see cref="IRepository{IProduct}"/> interface.
    /// </summary>
    public class ProductRepository : IRepository<IProduct>
    {
        private readonly string connectionString = @"Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        public ProductRepository()
        {
            this.connection = new SqlConnection(this.connectionString);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IProduct>> GetAllAsync()
        {
            var products = new List<IProduct>();
            this.connection.Open();

            using (SqlCommand selectCommand = new SqlCommand("SELECT * FROM Product WHERE IsActive = 1", this.connection))
            using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    products.Add(this.MapReaderToProduct(reader));
                }
            }

            this.connection.Close();
            return products;
        }

        /// <inheritdoc/>
        public async Task<IProduct> GetByIdAsync(long id)
        {
            this.connection.Open();

            using (SqlCommand selectCommand = new SqlCommand("SELECT * FROM Product WHERE IsActive = 1 AND ID = @ID", this.connection))
            {
                selectCommand.Parameters.AddWithValue("@ID", id);

                using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        IProduct product = this.MapReaderToProduct(reader);
                        this.connection.Close();
                        return product;
                    }
                }
            }

            this.connection.Close();
            return null;
        }

        /// <inheritdoc/>
        public async Task<IProduct> CreateAsync(IProduct entity)
        {
            this.connection.Open();

            using (SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO Product (Name, Price, Stock, CategoryID, Atributes, Size, Description, FileUrl, IsActive) " +
                "VALUES (@Name, @Price, @Stock, @CategoryID, @Atributes, @Size, @Description, @FileUrl, @IsActive)",
                this.connection))
            {
                this.FillInsertCommandParameters(insertCommand, entity);
                await insertCommand.ExecuteNonQueryAsync();
            }

            this.connection.Close();
            return entity;
        }

        /// <inheritdoc/>
        public async Task<IProduct> UpdateAsync(IProduct entity)
        {
            this.connection.Open();

            using (SqlCommand updateCommand = new SqlCommand(
                "UPDATE Product SET Name = @Name, Price = @Price, Description = @Description, " +
                "Stock = @Stock, FileUrl = @FileUrl, CategoryID = @CategoryID, Atributes = @Atributes, Size = @Size " +
                "WHERE ID = @ID;",
                this.connection))
            {
                updateCommand.Parameters.AddWithValue("@ID", entity.ID);
                updateCommand.Parameters.AddWithValue("@Name", entity.Name);
                updateCommand.Parameters.AddWithValue("@Price", entity.Price);
                updateCommand.Parameters.AddWithValue("@Stock", entity.Stock);
                updateCommand.Parameters.AddWithValue("@CategoryID", entity.CategoryID);
                updateCommand.Parameters.AddWithValue("@Description", entity.Description);
                updateCommand.Parameters.AddWithValue("@FileUrl", entity.FileUrl);

                if (entity.CategoryID == 1)
                {
                    updateCommand.Parameters.AddWithValue("@Atributes", ((ClothesProduct)entity).Attributes);
                    updateCommand.Parameters.AddWithValue("@Size", ((ClothesProduct)entity).Size);
                }
                else if (entity.CategoryID == 2)
                {
                    updateCommand.Parameters.AddWithValue("@Atributes", string.Empty);
                    updateCommand.Parameters.AddWithValue("@Size", ((FoodProduct)entity).Size);
                }
                else
                {
                    updateCommand.Parameters.AddWithValue("@Atributes", string.Empty);
                    updateCommand.Parameters.AddWithValue("@Size", string.Empty);
                }

                await updateCommand.ExecuteNonQueryAsync();
            }

            this.connection.Close();
            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(long id)
        {
            this.connection.Open();

            using (SqlCommand deleteCommand = new SqlCommand(
                "UPDATE Product SET IsActive = 0 WHERE ID = @ID", this.connection))
            {
                deleteCommand.Parameters.AddWithValue("@ID", id);
                await deleteCommand.ExecuteNonQueryAsync();
            }

            this.connection.Close();
            return true;
        }

        private IProduct MapReaderToProduct(SqlDataReader reader)
        {
            int category = reader.GetInt32(reader.GetOrdinal("CategoryID"));
            IProduct product = null;

            if (category == 1)
            {
                product = new ClothesProduct(
                    id: reader.GetInt32(reader.GetOrdinal("ID")),
                    name: reader.GetString(reader.GetOrdinal("Name")),
                    price: reader.GetDouble(reader.GetOrdinal("Price")),
                    stock: reader.GetInt32(reader.GetOrdinal("Stock")),
                    categoryId: category,
                    color: reader.GetString(reader.GetOrdinal("Atributes")),
                    size: reader.GetString(reader.GetOrdinal("Size")),
                    description: reader.GetString(reader.GetOrdinal("Description")),
                    fileUrl: reader.GetString(reader.GetOrdinal("FileUrl")),
                    isActive: true);
            }
            else if (category == 2)
            {
                product = new FoodProduct(
                    id: reader.GetInt32(reader.GetOrdinal("ID")),
                    name: reader.GetString(reader.GetOrdinal("Name")),
                    price: reader.GetDouble(reader.GetOrdinal("Price")),
                    stock: reader.GetInt32(reader.GetOrdinal("Stock")),
                    categoryId: category,
                    size: reader.GetString(reader.GetOrdinal("Size")),
                    description: reader.GetString(reader.GetOrdinal("Description")),
                    fileUrl: reader.GetString(reader.GetOrdinal("FileUrl")),
                    isActive: true);
            }
            else
            {
                product = new AccessoryProduct(
                    id: reader.GetInt32(reader.GetOrdinal("ID")),
                    name: reader.GetString(reader.GetOrdinal("Name")),
                    price: reader.GetDouble(reader.GetOrdinal("Price")),
                    stock: reader.GetInt32(reader.GetOrdinal("Stock")),
                    categoryId: category,
                    description: reader.GetString(reader.GetOrdinal("Description")),
                    fileUrl: reader.GetString(reader.GetOrdinal("FileUrl")),
                    isActive: true);
            }

            return product;
        }

        private void FillInsertCommandParameters(SqlCommand command, IProduct product)
        {
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Stock", product.Stock);
            command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@FileUrl", product.FileUrl);
            command.Parameters.AddWithValue("@IsActive", true);

            if (product.CategoryID == 1)
            {
                command.Parameters.AddWithValue("@Atributes", ((ClothesProduct)product).Attributes);
                command.Parameters.AddWithValue("@Size", ((ClothesProduct)product).Size);
            }
            else if (product.CategoryID == 2)
            {
                command.Parameters.AddWithValue("@Atributes", string.Empty);
                command.Parameters.AddWithValue("@Size", ((FoodProduct)product).Size);
            }
            else
            {
                command.Parameters.AddWithValue("@Atributes", string.Empty);
                command.Parameters.AddWithValue("@Size", string.Empty);
            }
        }
    }
}
