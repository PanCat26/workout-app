/*// <copyright file="ProductRepository.cs" company="WorkoutApp">
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
        private readonly string loginString = @"Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private readonly SqlConnection connection;
        private readonly List<IProduct> products;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// Opens the connection to the database and loads the initial data.
        /// </summary>
        public ProductRepository()
        {
            this.connection = new SqlConnection(this.loginString);
            this.products = new List<IProduct>();
            this.LoadData();
        }

        /// <summary>
        /// Gets the list of all products.
        /// </summary>
        /// <returns>A list of <see cref="IProduct"/> representing all products.</returns>
        public List<IProduct> GetProducts()
        {
            return this.products;
        }

        /// <summary>
        /// Gets the list of all products.
        /// </summary>
        /// <returns>A list of <see cref="IProduct"/> representing all products.</returns>
        public List<IProduct> GetAll()
        {
            return this.products;
        }

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The <see cref="IProduct"/> with the specified ID, or null if not found.</returns>
        public IProduct GetById(int id)
        {
            return this.products.Find(p => p.ID == id);
        }

        /// <summary>
        /// Asynchronously gets all products.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is a collection of <see cref="IProduct"/>.</returns>
        public async Task<IEnumerable<IProduct>> GetAllAsync()
        {
            return await Task.FromResult(this.products);
        }

        /// <summary>
        /// Asynchronously gets a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the <see cref="IProduct"/> with the specified ID, or null if not found.</returns>
        public async Task<IProduct> GetByIdAsync(int id)
        {
            return await Task.FromResult(this.products.Find(p => p.ID == id));
        }

        /// <summary>
        /// Loads all active products from the database.
        /// Clears the current list of products and repopulates it with the data from the database.
        /// </summary>
        public void LoadData()
        {
            this.products.Clear();
            this.connection.Open();

            SqlCommand selectCommand = new SqlCommand("SELECT * FROM Product WHERE IsActive = 1", this.connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            while (reader.Read())
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

                this.products.Add(product);
            }

            reader.Close();
            this.connection.Close();
        }

        /// <summary>
        /// Deletes a product by its ID by marking it as inactive.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        public void DeleteById(int productId)
        {
            this.connection.Open();

            SqlCommand deleteProductCommand = new SqlCommand(
                "UPDATE Product SET IsActive = 0 WHERE ID = @ID", this.connection);
            deleteProductCommand.Parameters.AddWithValue("@ID", productId);
            deleteProductCommand.ExecuteNonQuery();

            this.connection.Close();
            this.LoadData();
        }

        /// <summary>
        /// Updates the details of an existing product.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="name">The name of the product.</param>
        /// <param name="price">The price of the product.</param>
        /// <param name="stock">The stock quantity of the product.</param>
        /// <param name="categoryId">The category ID of the product.</param>
        /// <param name="description">The description of the product.</param>
        /// <param name="fileUrl">The file URL associated with the product.</param>
        /// <param name="color">The color of the product (if applicable).</param>
        /// <param name="size">The size of the product (if applicable).</param>
        public void UpdateProduct(
            int productId,
            string name,
            double price,
            int stock,
            int categoryId,
            string description,
            string fileUrl,
            string color,
            string size)
        {
            this.connection.Open();

            SqlCommand updateCommand = new SqlCommand(
                "UPDATE Product SET Name = @Name, Price = @Price, Description = @Description, " +
                "Stock = @Stock, FileUrl = @FileUrl, CategoryID = @CategoryID, Atributes = @Atributes, Size = @Size " +
                "WHERE ID = @ID;",
                this.connection);

            updateCommand.Parameters.AddWithValue("@ID", productId);
            updateCommand.Parameters.AddWithValue("@Name", name);
            updateCommand.Parameters.AddWithValue("@Price", price);
            updateCommand.Parameters.AddWithValue("@Stock", stock);
            updateCommand.Parameters.AddWithValue("@CategoryID", categoryId);
            updateCommand.Parameters.AddWithValue("@Atributes", color);
            updateCommand.Parameters.AddWithValue("@Size", size);
            updateCommand.Parameters.AddWithValue("@Description", description);
            updateCommand.Parameters.AddWithValue("@FileUrl", fileUrl);

            updateCommand.ExecuteNonQuery();

            this.connection.Close();
            this.LoadData();
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product to add.</param>
        public void AddProduct(IProduct product)
        {
            this.connection.Open();

            SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO Product (Name, Price, Stock, CategoryID, Atributes, Size, Description, FileUrl, IsActive) " +
                "VALUES (@Name, @Price, @Stock, @CategoryID, @Atributes, @Size, @Description, @FileUrl, @IsActive);",
                this.connection);

            this.FillInsertCommandParameters(insertCommand, product);

            insertCommand.ExecuteNonQuery();

            this.connection.Close();
            this.LoadData();
        }

        /// <summary>
        /// Asynchronously creates a new product and saves it to the database.
        /// </summary>
        /// <param name="entity">The product to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the created <see cref="IProduct"/>.</returns>
        public async Task<IProduct> CreateAsync(IProduct entity)
        {
            this.connection.Open();

            SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO Product (Name, Price, Stock, CategoryID, Atributes, Size, Description, FileUrl, IsActive) " +
                "VALUES (@Name, @Price, @Stock, @CategoryID, @Atributes, @Size, @Description, @FileUrl, @IsActive);",
                this.connection);

            this.FillInsertCommandParameters(insertCommand, entity);

            await insertCommand.ExecuteNonQueryAsync();

            this.connection.Close();
            this.LoadData();
            return entity;
        }

        /// <summary>
        /// Asynchronously updates an existing product.
        /// </summary>
        /// <param name="entity">The product entity to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the updated <see cref="IProduct"/>.</returns>
        public async Task<IProduct> UpdateAsync(IProduct entity)
        {
            this.connection.Open();

            SqlCommand updateCommand = new SqlCommand(
                "UPDATE Product SET Name = @Name, Price = @Price, Description = @Description, " +
                "Stock = @Stock, FileUrl = @FileUrl, CategoryID = @CategoryID, Atributes = @Atributes, Size = @Size " +
                "WHERE ID = @ID;",
                this.connection);

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

            this.connection.Close();
            this.LoadData();
            return entity;
        }

        /// <summary>
        /// Asynchronously deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if deletion is successful.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            this.connection.Open();

            SqlCommand deleteProductCommand = new SqlCommand(
                "UPDATE Product SET IsActive = 0 WHERE ID = @ID", this.connection);
            deleteProductCommand.Parameters.AddWithValue("@ID", id);

            await deleteProductCommand.ExecuteNonQueryAsync();

            this.connection.Close();
            this.LoadData();
            return true;
        }

        /// <summary>
        /// Populates the insert command parameters based on the product details.
        /// </summary>
        /// <param name="insertCommand">The SQL command to insert the product.</param>
        /// <param name="product">The product to add.</param>
        private void FillInsertCommandParameters(SqlCommand insertCommand, IProduct product)
        {
            insertCommand.Parameters.AddWithValue("@Name", product.Name);
            insertCommand.Parameters.AddWithValue("@Price", product.Price);
            insertCommand.Parameters.AddWithValue("@Stock", product.Stock);
            insertCommand.Parameters.AddWithValue("@CategoryID", product.CategoryID);

            if (product.CategoryID == 1)
            {
                insertCommand.Parameters.AddWithValue("@Atributes", ((ClothesProduct)product).Attributes);
                insertCommand.Parameters.AddWithValue("@Size", ((ClothesProduct)product).Size);
            }
            else if (product.CategoryID == 2)
            {
                insertCommand.Parameters.AddWithValue("@Atributes", string.Empty);
                insertCommand.Parameters.AddWithValue("@Size", ((FoodProduct)product).Size);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@Atributes", string.Empty);
                insertCommand.Parameters.AddWithValue("@Size", string.Empty);
            }

            insertCommand.Parameters.AddWithValue("@Description", product.Description);
            insertCommand.Parameters.AddWithValue("@FileUrl", product.FileUrl);
            insertCommand.Parameters.AddWithValue("@IsActive", 1);
        }
    }
}
*/