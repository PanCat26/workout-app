using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using Moq;

namespace WorkoutApp.Tests.Service
{
    public class OrderServiceTests : IDisposable
    {
        private readonly Mock<IRepository<Order>> mockRepository;
        private readonly OrderService orderService;

        public OrderServiceTests()
        {
            mockRepository = new Mock<IRepository<Order>>();
            orderService = new OrderService(mockRepository.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Call_Repository_And_Return_Order()
        {
            var order = new Order(1, new List<OrderItem>(), DateTime.Now);

            mockRepository.Setup(r => r.CreateAsync(order))
                          .ReturnsAsync(order);

            var result = await orderService.CreateAsync(order);

            Assert.Equal(order, result);
            mockRepository.Verify(r => r.CreateAsync(order), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Call_Repository_And_Return_True()
        {
            mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await orderService.DeleteAsync(1);

            Assert.True(result);
            mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_Call_Repository_And_Return_Orders()
        {
            var orders = new List<Order> { new Order(1, new List<OrderItem>(), DateTime.Now) };

            mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

            var result = await orderService.GetAllAsync();

            Assert.Single(result);
            mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Call_Repository_And_Return_Order()
        {
            var order = new Order(1, new List<OrderItem>(), DateTime.Now);

            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            var result = await orderService.GetByIdAsync(1);

            Assert.Equal(order, result);
            mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Call_Repository_And_Return_Updated_Order()
        {
            var updatedOrder = new Order(1, new List<OrderItem>(), DateTime.Now);

            mockRepository.Setup(r => r.UpdateAsync(updatedOrder)).ReturnsAsync(updatedOrder);

            var result = await orderService.UpdateAsync(updatedOrder);

            Assert.Equal(updatedOrder, result);
            mockRepository.Verify(r => r.UpdateAsync(updatedOrder), Times.Once);
        }

        public void Dispose()
        {

        }
    }
}
