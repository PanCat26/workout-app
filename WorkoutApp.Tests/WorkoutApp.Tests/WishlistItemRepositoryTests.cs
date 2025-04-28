using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WorkoutApp.Data.Database;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using Xunit;
using System.Configuration;


namespace WorkoutApp.Tests.Repository
{
    public class WishlistItemRepositoryTests
    {
        private readonly WishlistItemRepository repository;
        private readonly DbService dbService;

        public WishlistItemRepositoryTests()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            var connectionFactory = new SqlDbConnectionFactory(connectionString);

            this.dbService = new DbService(connectionFactory);
            this.repository = new WishlistItemRepository(this.dbService);
        }

        [Fact]
        public void GetAll_ShouldReturnWishlistItems()
        {
            var result = this.repository.GetAll();

            Assert.NotNull(result);
            // If database is empty, it will pass, or you can add fake data before running.
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnWishlistItems()
        {
            var result = await this.repository.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenItemDoesNotExist()
        {
            var result = await this.repository.GetByIdAsync(999999); // assume 999999 does not exist

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAndDeleteWishlistItem_ShouldWork()
        {
            // Arrange
            var wishlistItem = new WishlistItem
            {
                ProductID = 12345 // Use any product ID
            };

            // Act - create
            var createdItem = await this.repository.CreateAsync(wishlistItem);

            Assert.NotNull(createdItem);
            Assert.True(createdItem.ID > 0);

            // Act - delete
            var deleteResult = await this.repository.DeleteAsync(createdItem.ID);

            Assert.True(deleteResult);
        }

        [Fact]
        public void AddWishlistItem_ShouldCreateWishlistItem()
        {
            var result = this.repository.AddWishlistItem(12345);

            Assert.NotNull(result);
            Assert.Equal(12345, result.ProductID);
        }

        [Fact]
        public void DeleteById_ShouldDeleteWishlistItem()
        {
            // First create a new wishlist item to ensure it exists
            var createdItem = this.repository.AddWishlistItem(54321);

            var deleteResult = this.repository.DeleteById(createdItem.ID);

            Assert.True(deleteResult);
        }
    }
}
