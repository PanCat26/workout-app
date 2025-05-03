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
        private readonly Mock<ICartRepository> cartRepositoryMock;
        private readonly CartService cartService;
        private readonly SessionManager sessionManager;
        private readonly int customerID;

        public CartServiceTests()
        {
            cartRepositoryMock = new Mock<ICartRepository>();
            sessionManager = new SessionManager();
            customerID = sessionManager.CurrentUserId ?? 1;
            cartService = new CartService(cartRepositoryMock.Object);
        }

        [Fact]
        public async Task GetCartItems_ShouldReturnAllCartItems()
        {
            List<CartItem> items = new List<CartItem>
            {
                new CartItem(new Product(1, "Test Product1", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID, 2),
                new CartItem(new Product(2, "Test Product2", 9.99m, 10, new Category(1, "Category"), "S", "Blue", "", null), customerID, 1)
            };

            cartRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(items);

            List<CartItem> result = await cartService.GetCartItems();

            Assert.Equal(2, result.Count);
            cartRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCartItemById_ShouldReturnCorrectItem()
        {
            // Arrange
            int productId = 1;
            Product product = new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null);
            CartItem cartItem = new CartItem(product, customerID, 2);

            cartRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(cartItem);

            // Act
            var result = await cartService.GetCartItemById(productId);

            // Assert
            Assert.Equal(productId, result.Product.ID);
            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Product.Name);

            cartRepositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task IncreaseQuantity_ShouldIncrementAndUpdateItem()
        {
            int productId = 1;
            CartItem item = new CartItem(new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID, 2);

            await cartService.IncreaseQuantity(item);

            Assert.Equal(3, item.Quantity);
            cartRepositoryMock.Verify(repo => repo.UpdateAsync(item), Times.Once);
        }

        [Fact]
        public async Task DecreaseQuantity_ShouldDecrementAndUpdateItem()
        {
            int productId = 1;
            CartItem item = new CartItem(new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID, 2);

            await cartService.DecreaseQuantity(item);

            Assert.Equal(1, item.Quantity);
            cartRepositoryMock.Verify(repo => repo.UpdateAsync(item), Times.Once);
        }

        [Fact]
        public async Task DecreaseQuantity_ShouldDeleteItemIfQuantityIsZero()
        {
            int productId = 1;
            CartItem item = new CartItem(new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID, 1);

            await cartService.DecreaseQuantity(item);

            cartRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task RemoveCartItem_ShouldCallDeleteAsync()
        {
            int productId = 1;
            CartItem item = new CartItem(new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID, 2);

            await cartService.RemoveCartItem(item);

            cartRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task AddToCart_ShouldCallCreateAsyncWithCorrectParams()
        {
            int productId = 1;
            int quantity = 2;

            await cartService.AddToCart(productId, quantity);

            cartRepositoryMock.Verify(repo => repo.CreateAsync(productId, quantity), Times.Once);
        }

        [Fact]
        public async Task ResetCart_ShouldDeleteAllItems()
        {
            List<CartItem> items = new List<CartItem>
            {
                new CartItem(new Product(1, "Test Product1", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null), customerID, 2),
                new CartItem(new Product(2, "Test Product2", 9.99m, 10, new Category(1, "Category"), "S", "Blue", "", null), customerID, 1)
            };

            cartRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(items);

            await cartService.ResetCart();

            cartRepositoryMock.Verify(repo => repo.ResetCart(), Times.Once);
        }
    }
}
