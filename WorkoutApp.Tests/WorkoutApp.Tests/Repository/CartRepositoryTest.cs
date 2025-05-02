/*using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WorkoutApp.Data.Database;
using WorkoutApp.Infrastructure.Session;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using Xunit;

namespace WorkoutApp.Tests.Repository
{
    [Collection("DatabaseTests")]
    public class CartRepositoryTests : IDisposable
    {
        private readonly DbService dbService;
        private readonly SessionManager sessionManager;
        private readonly CartRepository cartRepository;

        private readonly int testCustomerId;
        private readonly int testProductId;

        public CartRepositoryTests()
        {
            string? testConnectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(testConnectionString))
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");

            DbConnectionFactory dbConnectionFactory = new(testConnectionString);
            dbService = new(dbConnectionFactory);
            sessionManager = new SessionManager();
            cartRepository = new CartRepository(dbService, sessionManager);

            try
            {
                // Reset identities
                string resetIdsQuery = @"
                DELETE FROM CartItem;
                DELETE FROM Product;
                DELETE FROM Category;
                DELETE FROM Customer;
                DBCC CHECKIDENT ('CartItem', RESEED, 0);
                DBCC CHECKIDENT ('Product', RESEED, 0);
                DBCC CHECKIDENT ('Category', RESEED, 0);
                DBCC CHECKIDENT ('Customer', RESEED, 0);
            ";
                dbService.ExecuteQueryAsync(resetIdsQuery, []).GetAwaiter().GetResult();

                // Insert test data
                testCustomerId = InsertTestCustomerAsync("Test User").GetAwaiter().GetResult();
                testProductId = InsertTestProductAsync("Test Product", 19.99m, 10).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database setup failed: " + ex.Message, ex);
            }

            // Set session
            sessionManager.CurrentUserId = testCustomerId;
        }

        public async void Dispose()
        {
            string cleanupQuery = @"
                DELETE FROM CartItem;
                DELETE FROM Product;
                DELETE FROM Category;
                DELETE FROM Customer;
                DBCC CHECKIDENT ('CartItem', RESEED, 0);
                DBCC CHECKIDENT ('Product', RESEED, 0);
                DBCC CHECKIDENT ('Category', RESEED, 0);
                DBCC CHECKIDENT ('Customer', RESEED, 0);
            ";

            try
            {
                dbService.ExecuteQueryAsync(cleanupQuery, []).GetAwaiter().GetResult();
                await Task.Delay(100); // minimal delay
                int remaining = await dbService.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM CartItem", []);
                Assert.Equal(0, remaining);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Test data cleanup failed: " + ex.Message, ex);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertCartItem()
        {
            await cartRepository.CreateAsync(productId: testProductId, quantity: 2);

            // Still using CartItem here because CreateAsync affects the cart state, not product structure
            Product? product = await cartRepository.GetByIdAsync(testProductId);

            Assert.NotNull(product);
            Assert.Equal(testProductId, product?.ID);
            Assert.Equal("Test Product", product?.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProductsInCart()
        {
            await cartRepository.CreateAsync(productId: testProductId, quantity: 1);

            IEnumerable<Product> products = await cartRepository.GetAllAsync();

            Assert.Single(products);
            Product product = Assert.Single(products); 
            Assert.Equal(testProductId, product.ID);
            Assert.Equal("Test Product", product.Name);
            Assert.Equal(19.99m, product.Price);
            Assert.Equal("M", product.Size);
            Assert.Equal("Red", product.Color);
            Assert.NotNull(product.Category);
            Assert.Equal("Test Category", product.Category.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyQuantity()
        {
            await cartRepository.CreateAsync(productId: testProductId, quantity: 1);
            CartItem itemToUpdate = new(testProductId, testCustomerId, 5);
            await cartRepository.UpdateAsync(itemToUpdate);

            Product? updatedProduct = await cartRepository.GetByIdAsync(testProductId);

            Assert.NotNull(updatedProduct);
            Assert.Equal(testProductId, updatedProduct?.ID);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveItem()
        {
            await cartRepository.CreateAsync(productId: testProductId, quantity: 1);
            bool deleted = await cartRepository.DeleteAsync(testProductId);

            Product? result = await cartRepository.GetByIdAsync(testProductId);

            Assert.True(deleted);
            Assert.Null(result);
        }

        [Fact]
        public async Task ResetCart_ShouldDeleteAllItems()
        {
            await cartRepository.CreateAsync(productId: testProductId, quantity: 3);
            bool result = await cartRepository.ResetCart();

            IEnumerable<Product> products = await cartRepository.GetAllAsync();

            Assert.True(result);
            Assert.Empty(products);
        }

        // Utility methods
        private async Task<int> InsertTestCustomerAsync(string name)
        {
            string query = "INSERT INTO Customer (Name) OUTPUT INSERTED.ID VALUES (@Name)";
            return await dbService.ExecuteScalarAsync<int>(query, [new SqlParameter("@Name", name)]);
        }

        private async Task<int> InsertTestProductAsync(string name, decimal price, int stock)
        {
            int categoryId = await InsertTestCategoryAsync("Test Category");

            string query = @"
                INSERT INTO Product (Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL)
                OUTPUT INSERTED.ID
                VALUES (@Name, @Price, @Stock, @CategoryID, 'M', 'Red', '', NULL)";
            List<SqlParameter> parameters = [
                new("@Name", name),
                new("@Price", price),
                new("@Stock", stock),
                new("@CategoryID", categoryId)
            ];
            return await dbService.ExecuteScalarAsync<int>(query, parameters);
        }

        private async Task<int> InsertTestCategoryAsync(string name)
        {
            string query = "INSERT INTO Category (Name) OUTPUT INSERTED.ID VALUES (@Name)";
            return await dbService.ExecuteScalarAsync<int>(query, [new SqlParameter("@Name", name)]);
        }
    }
}
*/