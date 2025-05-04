using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutApp.Models;
using WorkoutApp.Service;
using WorkoutApp.Utils.Filters;
using WorkoutApp.Repository;
using WorkoutApp.Data.Database;
using WorkoutApp.Infrastructure.Session;
using Xunit;

namespace WorkoutApp.Tests.Service
{
    [Collection("DatabaseTests")]
    public class IServiceTests
    {
        [Fact]
        public async Task GetFilteredAsync_ShouldReturnEmptyCollection()
        {
            // Arrange: use real or mock dependencies (real is fine since logic is stubbed)
            var dbService = new DbService(new DbConnectionFactory(""));
            var sessionManager = new SessionManager();
            var orderRepo = new OrderRepository(dbService, sessionManager);
            var cartRepo = new CartRepository(dbService, sessionManager);
            IService<Order> orderService = new OrderService(orderRepo, cartRepo);

            // Act
            var result = await orderService.GetFilteredAsync(new ProductFilter(null, null, null, null, null, null));

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}