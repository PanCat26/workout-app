using Moq;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using WorkoutApp.Data.Database;
using WorkoutApp.Infrastructure.Session;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using WorkoutApp.Utils.Filters;
using Xunit;

namespace WorkoutApp.Tests.Service
{
    [Collection("DatabaseTests")]
    public class CartServiceTests
    {
        private readonly Mock<IRepository<CartItem>> cartRepositoryMock;
        private readonly CartService cartService;
        private readonly SessionManager sessionManager;
        private readonly int customerID;

        public CartServiceTests()
        {
            cartRepositoryMock = new Mock<IRepository<CartItem>>();
            sessionManager = new SessionManager();
            customerID = sessionManager.CurrentUserId ?? 1;
            cartService = new CartService(cartRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCartItems()
        {
            List<CartItem> items = new List<CartItem>
            {
                new CartItem(1, new Product(1, "Test Product1", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID),
                new CartItem(2, new Product(2, "Test Product2", 9.99m, 10, new Category(1, "Category"), "S", "Blue", "", null), customerID)
            };

            cartRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(items);

            List<CartItem> result = (List<CartItem>)await cartService.GetAllAsync();

            Assert.Equal(2, result.Count);
            cartRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnItem_WhenItemExists()
        {
            // Arrange
            int productId = 1;
            int itemId = 1;
            Product product = new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null);
            CartItem cartItem = new CartItem(itemId, product, customerID);

            cartRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(cartItem);

            // Act
            var result = await cartService.GetByIdAsync(itemId);

            // Assert
            Assert.Equal(itemId, result.ID);
            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Product.Name);

            cartRepositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenItemDoesNotExist()
        {
            // Arrange
            int itemId = 1;
            cartRepositoryMock.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync((CartItem)null);
            // Act
            var result = await cartService.GetByIdAsync(itemId);
            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeletionSucceeds()
        {
            int productId = 1;
            int itemId = 1;
            CartItem item = new CartItem(itemId, new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID);
            cartRepositoryMock.Setup(repo => repo.DeleteAsync(itemId)).ReturnsAsync(true);

            bool result = await cartService.DeleteAsync(itemId);

            Assert.True(result);
            cartRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenDeletionFails()
        {
            bool result = await cartService.DeleteAsync(9999);

            Assert.False(result);
            cartRepositoryMock.Verify(repo => repo.DeleteAsync(9999), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallCreateAsyncWithCorrectParams()
        {
            int productId = 1;
            CartItem item = new CartItem(1, new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID);


            await cartService.CreateAsync(item);

            cartRepositoryMock.Verify(repo => repo.CreateAsync(item), Times.Once);
        }

        [Fact]
        public async Task ResetCart_ShouldDeleteAllItems()
        {
            List<CartItem> items = new List<CartItem>
            {
                new CartItem(1, new Product(1, "Test Product1", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID),
                new CartItem(2, new Product(2, "Test Product2", 9.99m, 10, new Category(1, "Category"), "S", "Blue", "", null), customerID)
            };

            cartRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(items);

            await cartService.ResetCart();

            cartRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
            cartRepositoryMock.Verify(repo => repo.DeleteAsync(2), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnSameEntity()
        {
            var product = new Product(1, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null);
            var cartItem = new CartItem(1, product, customerID);

            var result = await cartService.UpdateAsync(cartItem);

            Assert.Equal(cartItem, result);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldReturnEmptyList()
        {
            var mockFilter = new Mock<IFilter>();

            var result = await cartService.GetFilteredAsync(mockFilter.Object);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenRepositoryFails()
        {
            var product = new Product(1, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null);
            var cartItem = new CartItem(1, product, customerID);

            cartRepositoryMock.Setup(repo => repo.CreateAsync(cartItem)).ThrowsAsync(new System.Exception("Database error"));

            var exception = await Assert.ThrowsAsync<System.Exception>(() => cartService.CreateAsync(cartItem));

            Assert.Contains("Failed to add product", exception.Message);
        }

        [Fact]
        public async Task ResetCart_ShouldThrowException_WhenItemIdIsNull()
        {
            var cartItemWithNullId = new CartItem(null, new Product(1, "Product", 9.99m, 10, new Category(1, "Category"), "L", "Green", "", null), customerID);

            cartRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<CartItem> { cartItemWithNullId });

            await Assert.ThrowsAsync<Exception>(() => cartService.ResetCart());
        }

    }
}
