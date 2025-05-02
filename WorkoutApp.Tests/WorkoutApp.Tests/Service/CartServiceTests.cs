//using Moq;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using WorkoutApp.Infrastructure.Session;
//using WorkoutApp.Models;
//using WorkoutApp.Repository;
//using WorkoutApp.Service;
//using Xunit;

//namespace WorkoutApp.Tests.Service
//{
//    [Collection("DatabaseTests")]
//    public class CartServiceTests
//    {
//        private readonly Mock<IRepository<CartItem>> cartRepositoryMock;
//        private readonly Mock<IRepository<Product>> productRepositoryMock;
//        private readonly CartService cartService;
//        private readonly SessionManager sessionManager;
//        private readonly int customerID;

//        public CartServiceTests()
//        {
//            cartRepositoryMock = new Mock<IRepository<CartItem>>();
//            productRepositoryMock = new Mock<IRepository<Product>>();
//            sessionManager = new SessionManager();
//            customerID = sessionManager.CurrentUserId ?? 1; // Default fallback
//            cartService = new CartService(cartRepositoryMock.Object, productRepositoryMock.Object);
//        }

//        [Fact]
//        public async Task GetCartItems_ShouldReturnAllCartItems()
//        {
//            var items = new List<CartItem>
//            {
//                new CartItem(1, customerID, 2),
//                new CartItem(2, customerID, 1)
//            };

//            cartRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(items);

//            var result = await cartService.GetCartItems();

//            Assert.Equal(2, result.Count);
//            cartRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
//        }

//        [Fact]
//        public async Task GetCartItemById_ShouldReturnCorrectItem()
//        {
//            // Arrange
//            int productId = 1;
//            var product = new Product(productId, "Test Product", 9.99m, 10, new Category(1, "Category"), "M", "Red", "", null);

//            cartRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

//            // Act
//            var result = await cartService.GetCartItemById(productId);

//            // Assert
//            Assert.Equal(productId, result.ID);
//            Assert.NotNull(result);
//            Assert.Equal("Test Product", result.Name);

//            cartRepositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
//            productRepositoryMock.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
//        }

//        [Fact]
//        public async Task IncreaseQuantity_ShouldIncrementAndUpdateItem()
//        {
//            var item = new CartItem(1, customerID, 2);

//            await cartService.IncreaseQuantity(item);

//            Assert.Equal(3, item.Quantity);
//            cartRepositoryMock.Verify(repo => repo.UpdateAsync(item), Times.Once);
//        }

//        [Fact]
//        public async Task DecreaseQuantity_ShouldDecrementAndUpdateItem()
//        {
//            var item = new CartItem(1, customerID, 2);

//            await cartService.DecreaseQuantity(item);

//            Assert.Equal(1, item.Quantity);
//            cartRepositoryMock.Verify(repo => repo.UpdateAsync(item), Times.Once);
//        }

//        [Fact]
//        public async Task DecreaseQuantity_ShouldDeleteItemIfQuantityIsZero()
//        {
//            var item = new CartItem(1, customerID, 1);

//            await cartService.DecreaseQuantity(item);

//            cartRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
//        }

//        [Fact]
//        public async Task RemoveCartItem_ShouldCallDeleteAsync()
//        {
//            var item = new CartItem(1, customerID, 1);

//            await cartService.RemoveCartItem(item);

//            cartRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
//        }

//        [Fact]
//        public async Task AddToCart_ShouldCallCreateAsyncWithCorrectParams()
//        {
//            var product = new Product(1, "Test Product", 9.99m, 10, new Category(1, "Test"), "M", "Red", "", null);
//            productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

//            var cartItem = new CartItem(1, customerID, 2);

//            await cartService.AddToCart(cartItem);

//            cartRepositoryMock.Verify(repo => repo.CreateAsync(cartItem), Times.Once);
//        }

//        [Fact]
//        public async Task ResetCart_ShouldDeleteAllItems()
//        {
//            var items = new List<CartItem>
//            {
//                new CartItem(1, customerID, 2),
//                new CartItem(2, customerID, 1)
//            };
//            cartRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(items);

//            await cartService.ResetCart();

//            cartRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
//            cartRepositoryMock.Verify(repo => repo.DeleteAsync(2), Times.Once);
//        }
//    }
//}
