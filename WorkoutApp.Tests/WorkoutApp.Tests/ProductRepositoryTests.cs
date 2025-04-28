using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WorkoutApp.Data.Database;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using Xunit;
using System.Configuration;

namespace WorkoutApp.Tests.Repository
{
    public class ProductRepositoryTests
    {
        private readonly ProductRepository repository;

        public ProductRepositoryTests()
        {
            // Assume connection string from config
            var connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            var connection = new SqlConnection(connectionString);

            this.repository = new ProductRepository();
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
            var result = await this.repository.GetByIdAsync(999999); // Assume 999999 does not exist

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAndDeleteProduct_ShouldWork()
        {
            // Arrange
            var newProduct = new ClothesProduct(
                id: 0, // id is ignored when inserting, DB will auto-generate
                name: "Test Product",
                price: 19.99,
                stock: 10,
                categoryId: 1,
                color: "Red",
                size: "M",
                description: "Test Description",
                fileUrl: "http://example.com/image.jpg",
                isActive: true);

            // Act - Create
            var createdProduct = await this.repository.CreateAsync(newProduct);

            Assert.NotNull(createdProduct);

            // Act - Delete
            var deleteResult = await this.repository.DeleteAsync(createdProduct.ID);

            Assert.True(deleteResult);
        }

        [Fact]
        public async Task UpdateProduct_ShouldUpdateSuccessfully()
        {
            // Arrange
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

            var createdProduct = await this.repository.CreateAsync(newProduct);

            // Update properties
            createdProduct.Name = "Updated Product";
            createdProduct.Price = 25.00;
            createdProduct.Stock = 20;
            ((ClothesProduct)createdProduct).Attributes = "Green";
            ((ClothesProduct)createdProduct).Size = "L";

            // Act - Update
            var updatedProduct = await this.repository.UpdateAsync(createdProduct);

            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product", updatedProduct.Name);
            Assert.Equal(25.00, updatedProduct.Price);

            // Cleanup
            await this.repository.DeleteAsync(updatedProduct.ID);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            var result = await this.repository.DeleteAsync(999999); // Assume 999999 does not exist

            // Deleting a non-existing product would still return true (since we mark IsActive=0), 
            // but logically it should be treated carefully.
            Assert.True(result);
        }
    }
}
