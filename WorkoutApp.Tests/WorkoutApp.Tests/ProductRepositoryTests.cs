using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using Xunit;
using System.Configuration;
using WorkoutApp.Data.Database;

namespace WorkoutApp.Tests.Repository
{
    public class ProductRepositoryTests
    {
        private readonly ProductRepository repository;

        public ProductRepositoryTests()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            var dbConnectionFactory = new SqlDbConnectionFactory(connectionString);
            var dbService = new DbService(dbConnectionFactory);

            this.repository = new ProductRepository(dbService);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts()
        {
            var result = await this.repository.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var result = await this.repository.GetByIdAsync(999999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAndDeleteProduct_ShouldWork()
        {
            var newProduct = new ClothesProduct(
                id: 0,
                name: "Test Product",
                price: 19.99,
                stock: 10,
                categoryId: 1,
                color: "Red",
                size: "M",
                description: "Test Description",
                fileUrl: "http://example.com/image.jpg",
                isActive: true);

            await this.repository.CreateAsync(newProduct);

            var allProducts = await this.repository.GetAllAsync();
            var createdProduct = allProducts.FirstOrDefault(p => p.Name == newProduct.Name);

            Assert.NotNull(createdProduct);

            var deleteResult = await this.repository.DeleteAsync(createdProduct.ID);

            Assert.True(deleteResult);
        }

        [Fact]
        public async Task UpdateProduct_ShouldUpdateSuccessfully()
        {
            var newProduct = new ClothesProduct(
                id: 0,
                name: "Initial Product",
                price: 10.00,
                stock: 5,
                categoryId: 1,
                color: "Blue",
                size: "S",
                description: "Initial Description",
                fileUrl: "http://example.com/initial.jpg",
                isActive: true);

            await this.repository.CreateAsync(newProduct);

            var allProducts = await this.repository.GetAllAsync();
            var createdProduct = allProducts.FirstOrDefault(p => p.Name == newProduct.Name);

            Assert.NotNull(createdProduct);

            // Update properties
            createdProduct.Name = "Updated Product";
            createdProduct.Price = 25.00;
            createdProduct.Stock = 20;
            ((ClothesProduct)createdProduct).Attributes = "Green";
            ((ClothesProduct)createdProduct).Size = "L";

            var updatedProduct = await this.repository.UpdateAsync(createdProduct);

            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product", updatedProduct.Name);
            Assert.Equal(25.00, updatedProduct.Price);

            await this.repository.DeleteAsync(updatedProduct.ID);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var result = await this.repository.DeleteAsync(999999); // Try delete a non-existing product

            Assert.False(result); 
        }
    }
}
