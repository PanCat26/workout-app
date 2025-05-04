using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutApp.Infrastructure.Session;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using Xunit;

namespace WorkoutApp.Tests.Service
{
    [Collection("DatabaseTests")]
    public class WishlistServiceTests
    {
        private readonly Mock<IRepository<WishlistItem>> wishlistRepositoryMock;
        private readonly WishlistService wishlistService;
        private readonly SessionManager sessionManager;
        private readonly int customerID;

        public WishlistServiceTests()
        {
            wishlistRepositoryMock = new Mock<IRepository<WishlistItem>>();
            sessionManager = new SessionManager();
            customerID = sessionManager.CurrentUserId ?? 1;
            wishlistService = new WishlistService(wishlistRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllWishlistItems()
        {
            List<WishlistItem> items = new List<WishlistItem>
            {
                new WishlistItem(1, new Product(1, "Test Product1", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID),
                new WishlistItem(2, new Product(2, "Test Product2", 9.99m, 10, new Category(1, "Category"), "S", "Blue", "", null), customerID)
            };

            wishlistRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(items);

            List<WishlistItem> result = (List<WishlistItem>)await wishlistService.GetAllAsync();

            Assert.Equal(2, result.Count);
            wishlistRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectItem()
        {
            int itemId = 1;
            Product product = new Product(1, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null);
            WishlistItem wishlistItem = new WishlistItem(itemId, product, customerID);

            wishlistRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(wishlistItem);

            WishlistItem result = await wishlistService.GetByIdAsync(itemId);

            Assert.NotNull(result);
            Assert.Equal(itemId, result.ID);
            Assert.Equal("Test Product", result.Product.Name);

            wishlistRepositoryMock.Verify(repo => repo.GetByIdAsync(itemId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAsync()
        {
            int itemId = 1;
            WishlistItem item = new WishlistItem(1, new Product(1, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID);

            await wishlistService.DeleteAsync(itemId);

            wishlistRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallCreateAsyncWithCorrectParams()
        {
            WishlistItem item = new WishlistItem(null, new Product(1, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID);

            await wishlistService.CreateAsync(item);

            wishlistRepositoryMock.Verify(repo => repo.CreateAsync(item), Times.Once);
        }

        [Fact]
        public async Task ResetWishlist_ShouldDeleteAllItems()
        {
            List<WishlistItem> items = new List<WishlistItem>
            {
                new WishlistItem(1, new Product(1, "Test Product1", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID),
                new WishlistItem(2, new Product(2, "Test Product2", 9.99m, 10, new Category(1, "Category"), "S", "Blue", "", null), customerID)
            };

            wishlistRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(items);

            await wishlistService.ResetWishlist();

            wishlistRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
            wishlistRepositoryMock.Verify(repo => repo.DeleteAsync(2), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSameWishlistItem()
        {
            var wishlistItem = new WishlistItem(1, new Product(1, "P", 9.99m, 10, new Category(1, "C"), "M", "Red", "", null), customerID);

            var result = await wishlistService.UpdateAsync(wishlistItem);

            Assert.Equal(wishlistItem, result);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldReturnEmptyList()
        {
            var result = await wishlistService.GetFilteredAsync(Mock.Of<WorkoutApp.Utils.Filters.IFilter>());

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowWrappedException_WhenRepositoryThrows()
        {
            wishlistRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                                  .ThrowsAsync(new Exception("DB failure"));

            var ex = await Assert.ThrowsAsync<Exception>(() => wishlistService.GetByIdAsync(1));

            Assert.Contains("Failed to retrieve wishlist item", ex.Message);
            Assert.IsType<Exception>(ex.InnerException);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowWrappedException_WhenRepositoryThrows()
        {
            wishlistRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<int>()))
                                  .ThrowsAsync(new Exception("DB failure"));

            var ex = await Assert.ThrowsAsync<Exception>(() => wishlistService.DeleteAsync(1));

            Assert.Contains("Failed to remove wishlist item", ex.Message);
        }
    }
}
