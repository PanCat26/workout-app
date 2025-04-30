using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using WorkoutApp.Data.Database;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;

namespace WorkoutApp.Tests.Service
{
    [Collection("DatabaseTests")]
    public class OrderServiceTests : IDisposable
    {
        private readonly OrderService service;
        private readonly DbConnectionFactory connectionFactory;
        private readonly DbService dbService;
        private readonly OrderRepository orderRepository;

        public OrderServiceTests()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            connectionFactory = new SqlDbConnectionFactory(connectionString);
            dbService = new DbService(connectionFactory);
            orderRepository = new OrderRepository(dbService);
            service = new OrderService(orderRepository);

            using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
            try
            {
                connection.Open();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to open SQL connection: {exception}");
                throw;
            }

            using SqlCommand insertCustomerCommand = new(
                "INSERT INTO Customer (IsActive) VALUES (1)", connection);
            using SqlCommand insertOrderCommand = new(
                "INSERT INTO [Order] (CustomerId, OrderDate, TotalAmount, IsActive) VALUES (1, GETDATE(), 100, 1), (1, GETDATE(), 200, 0)", connection);

            try
            {
                insertCustomerCommand.ExecuteNonQuery();
                insertOrderCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to insert test data: {exception}");
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task Test_GetAllAsync()
        {
            var orders = await service.GetAllAsync();
            Assert.NotNull(orders);
            Assert.Equal(2, orders.Count());
        }

        [Fact]
        public async Task Test_GetByIdAsync()
        {
            var order1 = await service.GetByIdAsync(1);
            Assert.NotNull(order1);
            Assert.Equal(1, order1.ID);

            var order2 = await service.GetByIdAsync(1001);
            Assert.Null(order2);
        }

        [Fact]
        public async Task Test_CreateAsync()
        {
            var initialOrders = await service.GetAllAsync();
            Assert.NotNull(initialOrders);
            Assert.Equal(2, initialOrders.Count());

            Order order = new Order(3, 1, DateTime.Now, 9999, false);
            var result = await service.CreateAsync(order);
            Assert.NotNull(result);

            var finalOrders = await service.GetAllAsync();
            Assert.NotNull(finalOrders);
            Assert.Equal(3, finalOrders.Count());

            var insertedOrder = await service.GetByIdAsync(3);
            Assert.NotNull(insertedOrder);

            Assert.Equal(order.ID, insertedOrder.ID);
            Assert.Equal(order.CustomerID, insertedOrder.CustomerID);
            Assert.Equal(order.TotalAmount, insertedOrder.TotalAmount);
            Assert.Equal(order.IsActive, insertedOrder.IsActive);
        }

        [Fact]
        public async Task Test_UpdateAsync()
        {
            var initialOrder = await service.GetByIdAsync(1);
            Assert.NotNull(initialOrder);

            Order orderToUpdate = new Order(initialOrder.ID, initialOrder.CustomerID, initialOrder.OrderDate, 0, !initialOrder.IsActive);
            await service.UpdateAsync(orderToUpdate);

            var finalOrder = await service.GetByIdAsync(1);
            Assert.NotNull(finalOrder);
            Assert.Equal(initialOrder.ID, finalOrder.ID);
            Assert.Equal(initialOrder.CustomerID, finalOrder.CustomerID);
            Assert.Equal(initialOrder.OrderDate, finalOrder.OrderDate);
            Assert.Equal(0, finalOrder.TotalAmount);
            Assert.Equal(!initialOrder.IsActive, finalOrder.IsActive);
        }

        [Fact]
        public async Task Test_DeleteAsync()
        {
            var initialOrder = await service.GetByIdAsync(1);
            Assert.NotNull(initialOrder);
            Assert.True(initialOrder.IsActive);

            await service.DeleteAsync(1);

            var finalOrder = await service.GetByIdAsync(1);
            Assert.NotNull(finalOrder);
            Assert.False(finalOrder.IsActive);
        }

        [Fact]
        public async Task Test_SendOrder()
        {
            var initialOrders = await service.GetAllAsync();
            Assert.NotNull(initialOrders);
            var initialCount = initialOrders.Count();

            double totalAmount = 150.0;
            service.SendOrder(totalAmount);

            // Wait a bit for the async operation to complete
            await Task.Delay(100);

            var finalOrders = await service.GetAllAsync();
            Assert.NotNull(finalOrders);
            Assert.Equal(initialCount + 1, finalOrders.Count());

            var newOrder = finalOrders.OrderByDescending(o => o.ID).First();
            Assert.Equal(totalAmount, newOrder.TotalAmount);
            Assert.True(newOrder.IsActive);
            Assert.Equal(1, newOrder.CustomerID);
        }

        public void Dispose()
        {
            using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
            try
            {
                connection.Open();
                using SqlCommand deleteOrderCommand = new("DELETE FROM [Order]", connection);
                using SqlCommand deleteCustomerCommand = new("DELETE FROM Customer", connection);
                deleteOrderCommand.ExecuteNonQuery();
                deleteCustomerCommand.ExecuteNonQuery();

                using SqlCommand resetIdentityCustomer = new("DBCC CHECKIDENT ('[Customer]', RESEED, 0)", connection);
                using SqlCommand resetIdentityOrder = new("DBCC CHECKIDENT ('[Order]', RESEED, 0)", connection);
                resetIdentityCustomer.ExecuteNonQuery();
                resetIdentityOrder.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to delete test data: {exception}");
                throw;
            }
            finally
            {
                connection.Close();
                GC.SuppressFinalize(this);
            }
        }
    }
}
