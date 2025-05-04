using Moq;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using WorkoutApp.Data.Database;
using WorkoutApp.Infrastructure.Session;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;
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
        public async Task GetCartItems_ShouldReturnAllCartItems()
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
        public async Task GetCartItemById_ShouldReturnCorrectItem()
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
        public async Task RemoveCartItem_ShouldCallDeleteAsync()
        {
            int productId = 1;
            int itemId = 1;
            CartItem item = new CartItem(itemId, new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID);

            await cartService.DeleteAsync(itemId);

            cartRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task AddToCart_ShouldCallCreateAsyncWithCorrectParams()
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
    }
}
