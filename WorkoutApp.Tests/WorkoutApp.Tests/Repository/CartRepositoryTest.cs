using System;
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
        private readonly int testCategoryId;

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
                DBCC CHECKIDENT ('Product', RESEED, 0);
                DBCC CHECKIDENT ('Category', RESEED, 0);
                DBCC CHECKIDENT ('Customer', RESEED, 0);
            ";
                dbService.ExecuteQueryAsync(resetIdsQuery, new List<SqlParameter>()).GetAwaiter().GetResult();

                // Insert test data
                testCustomerId = sessionManager.CurrentUserId ?? 1;
                InsertTestCustomerAsync("Test User").GetAwaiter().GetResult();
                testCategoryId = InsertTestCategoryAsync("Test Category").GetAwaiter().GetResult();
                testProductId = InsertTestProductAsync("Test Product", 19.99m, 10, testCategoryId).GetAwaiter().GetResult();
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
                DBCC CHECKIDENT ('Product', RESEED, 0);
                DBCC CHECKIDENT ('Category', RESEED, 0);
                DBCC CHECKIDENT ('Customer', RESEED, 0);
            ";

            try
            {
                dbService.ExecuteQueryAsync(cleanupQuery, new List<SqlParameter>()).GetAwaiter().GetResult();
                await Task.Delay(100); // minimal delay
                int remaining = await dbService.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM CartItem", new List<SqlParameter>());
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
            CartItem cartItem = new(
                id: 0, // initial value doesn't matter
                product: new Product(testProductId, "Test Product", 19.99m, 10, new Category(testCategoryId, "Test Category"), "M", "Red", "Sample description", null),
                customerID: testCustomerId);

            CartItem createdItem = await cartRepository.CreateAsync(cartItem); // this sets ID after DB insert

            CartItem? retrievedItem = await cartRepository.GetByIdAsync((int)createdItem.ID);

            Assert.NotNull(retrievedItem);
            Assert.Equal(createdItem.ID, retrievedItem.ID);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnCartItems()
        {
            CartItem cartItem = new(1,new Product(testProductId, "Test Product", 19.99m, 10, new Category(testCategoryId, "Test Category"), "M", "Red", "Sample description", null), testCustomerId);
            await cartRepository.CreateAsync(cartItem);

            IEnumerable<CartItem> cartItems = await cartRepository.GetAllAsync();

            CartItem item = Assert.Single(cartItems);
            Assert.Equal(testProductId, item.Product.ID);
            Assert.Equal(testCustomerId, item.CustomerID);

            Product product = item.Product;
            Assert.Equal("Test Product", product.Name);
            Assert.Equal(19.99m, product.Price);
            Assert.Equal("M", product.Size);
            Assert.Equal("Red", product.Color);
            Assert.NotNull(product.Category);
            Assert.Equal("Test Category", product.Category.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmpty_WhenNoItemsExist()
        {
            IEnumerable<CartItem> items = await cartRepository.GetAllAsync();

            Assert.Empty(items);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenItemExists()
        {
            int cartItemId = 1;
            CartItem cartItem = new(cartItemId,new Product(testProductId, "Test Product", 19.99m, 10, new Category(testCategoryId, "Test Category"), "M", "Red", "Sample description", null), testCustomerId);
            CartItem returnedItem = await cartRepository.CreateAsync(cartItem);


            bool deleted = await cartRepository.DeleteAsync((int)returnedItem.ID);

            CartItem? result = await cartRepository.GetByIdAsync((int)returnedItem.ID);

            Assert.True(deleted);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenItemDoesNotExists()
        {
            bool deleted = await cartRepository.DeleteAsync(9999);

            Assert.False(deleted);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnItem_WhenItemExists()
        {
            CartItem cartItem = new(
                id: null, 
                product: new Product(testProductId, "Test Product", 19.99m, 10, new Category(testCategoryId, "Test Category"), "M", "Red", "Sample description", null),
                customerID: testCustomerId);

            CartItem createdItem = await cartRepository.CreateAsync(cartItem);
            CartItem? result = await cartRepository.GetByIdAsync((int)createdItem.ID);

            Assert.NotNull(result);
            Assert.Equal(createdItem.ID, result.ID);
            Assert.Equal(testCustomerId, result.CustomerID);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenItemDoesNotExist()
        {
            CartItem? result = await cartRepository.GetByIdAsync(999999); // Non-existing product ID

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSameCartItem()
        {
            CartItem cartItem = new(
                id: null,
                product: new Product(testProductId, "Test Product", 19.99m, 10, new Category(testCategoryId, "Test Category"), "M", "Red", "Sample description", null),
                customerID: testCustomerId);

            CartItem createdItem = await cartRepository.CreateAsync(cartItem);

            CartItem updatedItem = await cartRepository.UpdateAsync(createdItem);

            Assert.Equal(createdItem.ID, updatedItem.ID);
            Assert.Equal(createdItem.Product.ID, updatedItem.Product.ID);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenSessionUserIdIsNull()
        {
            sessionManager.CurrentUserId = null;

            CartItem cartItem = new(
                id: null,
                product: new Product(testProductId, "Test Product", 19.99m, 10, new Category(testCategoryId, "Test Category"), "M", "Red", "Sample description", null),
                customerID: 0);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                cartRepository.CreateAsync(cartItem));
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrow_WhenSessionUserIdIsNull()
        {
            sessionManager.CurrentUserId = null;

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                cartRepository.GetAllAsync());
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleMultipleInsertions()
        {
            var tasks = new List<Task<CartItem>>();

            for (int i = 0; i < 5; i++)
            {
                CartItem item = new(
                    id: null,
                    product: new Product(testProductId, "Test Product", 19.99m, 10, new Category(testCategoryId, "Test Category"), "M", "Red", "Sample description", null),
                    customerID: testCustomerId);

                tasks.Add(cartRepository.CreateAsync(item));
            }

            await Task.WhenAll(tasks);

            IEnumerable<CartItem> allItems = await cartRepository.GetAllAsync();
            Assert.True(allItems.Count() >= 5);
        }

        private async Task InsertTestCustomerAsync(string name)
        {
            string query = "INSERT INTO Customer (Name) VALUES (@Name);";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name },
            };

            await dbService.ExecuteQueryAsync(query, parameters);
        }

        private async Task<int> InsertTestCategoryAsync(string name)
        {
            string query = "INSERT INTO Category (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name }
            };
            return await dbService.ExecuteScalarAsync<int>(query, parameters);
        }

        private async Task<int> InsertTestProductAsync(string name, decimal price, int stock, int categoryId)
        {
            string query = @"
            INSERT INTO Product (Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL) 
            VALUES (@Name, @Price, @Stock, @CategoryID, @Size, @Color, @Description, @PhotoURL); 
            SELECT SCOPE_IDENTITY();";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@Price", SqlDbType.Decimal) { Value = price },
                new SqlParameter("@Stock", SqlDbType.Int) { Value = stock },
                new SqlParameter("@CategoryID", SqlDbType.Int) { Value = categoryId },
                new SqlParameter("@Size", SqlDbType.NVarChar) { Value = "M" }, 
                new SqlParameter("@Color", SqlDbType.NVarChar) { Value = "Red" },   
                new SqlParameter("@Description", SqlDbType.NVarChar) { Value = "Sample description" }, 
                new SqlParameter("@PhotoURL", SqlDbType.NVarChar) { Value = "shirt.jpg" }
            };
            return await dbService.ExecuteScalarAsync<int>(query, parameters);
        }
    }
}