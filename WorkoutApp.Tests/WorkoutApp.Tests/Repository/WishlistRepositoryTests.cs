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
    public class WishlistItemRepositoryTests : IDisposable
    {
        private readonly DbService dbService;
        private readonly SessionManager sessionManager;
        private readonly WishlistItemRepository wishlistRepository;

        private readonly int testCustomerId;
        private readonly int testProductId;
        private readonly int testCategoryId;

        public WishlistItemRepositoryTests()
        {
            string? testConnectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(testConnectionString))
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");

            DbConnectionFactory dbConnectionFactory = new(testConnectionString);
            dbService = new(dbConnectionFactory);
            sessionManager = new SessionManager();
            wishlistRepository = new WishlistItemRepository(dbService, sessionManager);

            try
            {
                string resetQuery = @"
                    DELETE FROM WishlistItem;
                    DELETE FROM Product;
                    DELETE FROM Category;
                    DELETE FROM Customer;
                    DBCC CHECKIDENT ('WishlistItem', RESEED, 0);
                    DBCC CHECKIDENT ('Product', RESEED, 0);
                    DBCC CHECKIDENT ('Category', RESEED, 0);
                    DBCC CHECKIDENT ('Customer', RESEED, 0);";

                dbService.ExecuteQueryAsync(resetQuery, new List<SqlParameter>()).GetAwaiter().GetResult();

                testCustomerId = sessionManager.CurrentUserId ?? 1;
                InsertTestCustomerAsync("Wishlist User").GetAwaiter().GetResult();
                testCategoryId = InsertTestCategoryAsync("Wishlist Category").GetAwaiter().GetResult();
                testProductId = InsertTestProductAsync("Wishlist Product", 29.99m, 15, testCategoryId).GetAwaiter().GetResult();

                sessionManager.CurrentUserId = testCustomerId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Test setup failed: " + ex.Message, ex);
            }
        }

        public async void Dispose()
        {
            string cleanupQuery = @"
                DELETE FROM WishlistItem;
                DELETE FROM Product;
                DELETE FROM Category;
                DELETE FROM Customer;
                DBCC CHECKIDENT ('WishlistItem', RESEED, 0);
                DBCC CHECKIDENT ('Product', RESEED, 0);
                DBCC CHECKIDENT ('Category', RESEED, 0);
                DBCC CHECKIDENT ('Customer', RESEED, 0);";

            try
            {
                dbService.ExecuteQueryAsync(cleanupQuery, new List<SqlParameter>()).GetAwaiter().GetResult();
                await Task.Delay(100);
                int remaining = await dbService.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM WishlistItem", new List<SqlParameter>());
                Assert.Equal(0, remaining);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Cleanup failed: " + ex.Message, ex);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertWishlistItem()
        {
            WishlistItem wishlistItem = new(
                id: null,
                product: new Product(testProductId, "Wishlist Product", 29.99m, 15, new Category(testCategoryId, "Wishlist Category"), "L", "Blue", "Wishlist description", null),
                customerID: testCustomerId);

            WishlistItem createdItem = await wishlistRepository.CreateAsync(wishlistItem);

            WishlistItem? retrievedItem = await wishlistRepository.GetByIdAsync((int)createdItem.ID);

            Assert.NotNull(retrievedItem);
            Assert.Equal(createdItem.ID, retrievedItem.ID);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnWishlistItems()
        {
            WishlistItem wishlistItem = new(
                id: 0,
                product: new Product(testProductId, "Wishlist Product", 29.99m, 15, new Category(testCategoryId, "Wishlist Category"), "L", "Blue", "Wishlist description", null),
                customerID: testCustomerId);

            await wishlistRepository.CreateAsync(wishlistItem);

            IEnumerable<WishlistItem> wishlistItems = await wishlistRepository.GetAllAsync();

            WishlistItem item = Assert.Single(wishlistItems);
            Assert.Equal(testProductId, item.Product.ID);
            Assert.Equal(testCustomerId, item.CustomerID);

            Product product = item.Product;
            Assert.Equal("Wishlist Product", product.Name);
            Assert.Equal(29.99m, product.Price);
            Assert.Equal("L", product.Size);
            Assert.Equal("Blue", product.Color);
            Assert.NotNull(product.Category);
            Assert.Equal("Wishlist Category", product.Category.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenWishlistItemExists()
        {
            WishlistItem wishlistItem = new(
                id: 0,
                product: new Product(testProductId, "Wishlist Product", 29.99m, 15, new Category(testCategoryId, "Wishlist Category"), "L", "Blue", "Wishlist description", null),
                customerID: testCustomerId);

            WishlistItem createdItem = await wishlistRepository.CreateAsync(wishlistItem);

            bool deleted = await wishlistRepository.DeleteAsync((int)createdItem.ID);
            WishlistItem? result = await wishlistRepository.GetByIdAsync((int)createdItem.ID);

            Assert.True(deleted);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenWishlistItemDoesNotExists()
        {
            bool deleted = await wishlistRepository.DeleteAsync(9999);

            Assert.False(deleted);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnWishlistItem_WhenItemExists()
        {
            WishlistItem wishlistItem = new(
                id: null,
                product: new Product(testProductId, "Wishlist Product", 29.99m, 15, new Category(testCategoryId, "Wishlist Category"), "L", "Blue", "Wishlist description", null),
                customerID: testCustomerId);

            WishlistItem createdItem = await wishlistRepository.CreateAsync(wishlistItem);
            WishlistItem? result = await wishlistRepository.GetByIdAsync((int)createdItem.ID);

            Assert.NotNull(result);
            Assert.Equal(createdItem.ID, result.ID);
            Assert.Equal(testCustomerId, result.CustomerID);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenItemDoesNotExists()
        {
            WishlistItem? result = await wishlistRepository.GetByIdAsync(9999);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenSessionUserIdIsNull()
        {
            sessionManager.CurrentUserId = null;

            WishlistItem wishlistItem = new(
                id: null,
                product: new Product(testProductId, "Wishlist Product", 29.99m, 15, new Category(testCategoryId, "Wishlist Category"), "L", "Blue", "Wishlist description", null),
                customerID: testCustomerId);

            await Assert.ThrowsAsync<InvalidOperationException>(() => wishlistRepository.CreateAsync(wishlistItem));
        }

        [Fact]
        public async Task GetAllAsync_ShouldThrow_WhenSessionUserIdIsNull()
        {
            sessionManager.CurrentUserId = null;
            await Assert.ThrowsAsync<InvalidOperationException>(() => wishlistRepository.GetAllAsync());
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSameInstance()
        {
            WishlistItem wishlistItem = new(
                id: 1,
                product: new Product(testProductId, "Wishlist Product", 29.99m, 15, new Category(testCategoryId, "Wishlist Category"), "L", "Blue", "Wishlist description", null),
                customerID: testCustomerId);

            WishlistItem result = await wishlistRepository.UpdateAsync(wishlistItem);

            Assert.Same(wishlistItem, result);
        }


        private async Task InsertTestCustomerAsync(string name)
        {
            string query = "INSERT INTO Customer (Name) VALUES (@Name);";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name }
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
                new SqlParameter("@Size", SqlDbType.NVarChar) { Value = "L" },
                new SqlParameter("@Color", SqlDbType.NVarChar) { Value = "Blue" },
                new SqlParameter("@Description", SqlDbType.NVarChar) { Value = "Wishlist description" },
                new SqlParameter("@PhotoURL", SqlDbType.NVarChar) { Value = "wishlist.jpg" }
            };

            return await dbService.ExecuteScalarAsync<int>(query, parameters);
        }
    }
}
