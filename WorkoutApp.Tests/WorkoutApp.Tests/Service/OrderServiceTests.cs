using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using Moq;

namespace WorkoutApp.Tests.Service
{
    public class OrderServiceTests
    {
        private readonly Mock<IRepository<Order>> mockOrderRepository;
        private readonly Mock<IRepository<CartItem>> mockCartRepository;
        private readonly OrderService orderService;

        public OrderServiceTests()
        {
            mockOrderRepository = new Mock<IRepository<Order>>();
            mockCartRepository = new Mock<IRepository<CartItem>>();
            orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Call_Repository_And_Return_Order()
        {
            var order = new Order(1, new List<OrderItem>(), DateTime.Now);

            mockOrderRepository.Setup(r => r.CreateAsync(order))
                          .ReturnsAsync(order);

            var result = await orderService.CreateAsync(order);

            Assert.Equal(order, result);
            mockOrderRepository.Verify(r => r.CreateAsync(order), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Call_Repository_And_Return_True()
        {
            mockOrderRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await orderService.DeleteAsync(1);

            Assert.True(result);
            mockOrderRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_Call_Repository_And_Return_Orders()
        {
            var orders = new List<Order> { new Order(1, new List<OrderItem>(), DateTime.Now) };

            mockOrderRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

            var result = await orderService.GetAllAsync();

            Assert.Single(result);
            mockOrderRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Call_Repository_And_Return_Order()
        {
            var order = new Order(1, new List<OrderItem>(), DateTime.Now);

            mockOrderRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            var result = await orderService.GetByIdAsync(1);

            Assert.Equal(order, result);
            mockOrderRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Call_Repository_And_Return_Updated_Order()
        {
            var updatedOrder = new Order(1, new List<OrderItem>(), DateTime.Now);

            mockOrderRepository.Setup(r => r.UpdateAsync(updatedOrder)).ReturnsAsync(updatedOrder);

            var result = await orderService.UpdateAsync(updatedOrder);

            Assert.Equal(updatedOrder, result);
            mockOrderRepository.Verify(r => r.UpdateAsync(updatedOrder), Times.Once);
        }

        [Fact]
        public async Task CreateOrderFromCartAsync_CreatesOrderAndDeletesCartItems()
        {
            // Arrange
            var mockOrderRepo = new Mock<IRepository<Order>>();
            var mockCartRepo = new Mock<IRepository<CartItem>>();

            var sampleCartItems = new List<CartItem>
            {
                new CartItem(new Product(1, "Mat", 20.0m, 10, new Category(1, "Gear"), "M", "Black", "Yoga mat", null),1, 2),
                new CartItem(new Product(2, "Dumbbell", 50.0m, 5, new Category(2, "Weights"), "L", "Silver", "Iron dumbbell", null),1, 1)
            };

            mockCartRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(sampleCartItems);
            mockCartRepo.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);
            mockOrderRepo.Setup(repo => repo.CreateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

            var service = new OrderService(mockOrderRepo.Object, mockCartRepo.Object);

            // Act
            await service.CreateOrderFromCartAsync();

            // Assert
            mockCartRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
            foreach (var item in sampleCartItems)
            {
                mockCartRepo.Verify(repo => repo.DeleteAsync((int)item.Product.ID), Times.Once);
            }

            mockOrderRepo.Verify(repo => repo.CreateAsync(It.Is<Order>(o =>
                o.OrderItems.Count() == 2 &&
                o.OrderItems.Any(i => i.Product.ID == 1 && i.Quantity == 2) &&
                o.OrderItems.Any(i => i.Product.ID == 2 && i.Quantity == 1)
            )), Times.Once);
        }
    }
}
