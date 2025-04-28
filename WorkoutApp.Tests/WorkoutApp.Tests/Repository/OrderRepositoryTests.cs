using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using WorkoutApp.Data.Database;
using WorkoutApp.Models;
using WorkoutApp.Repository;

namespace WorkoutApp.Tests.Repository
{
    public class OrderRepositoryTests : IDisposable
    {
        // all tests are running on the assumption that all the tables are empty
        private readonly OrderRepository repository;
        private readonly DbConnectionFactory connectionFactory;
        private readonly DbService dbService;

        public OrderRepositoryTests()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            connectionFactory = new SqlDbConnectionFactory(connectionString);
            dbService = new DbService(connectionFactory);
            repository = new OrderRepository(dbService);

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

            using SqlCommand resetIdentityCusotmer = new("DBCC CHECKIDENT ('Customer', RESEED, 0)", connection);
            using SqlCommand resetIdentityOrder = new("DBCC CHECKIDENT ('Order', RESEED, 0)", connection);
            using SqlCommand insertCustomerCommand = new(
                "INSERT INTO Customer (IsActive) VALUES (1)", connection);
            using SqlCommand insertOrderCommand = new(
                "INSERT INTO [Order] (CustomerId, OrderDate, TotalAmount, IsActive) VALUES (1, GETDATE(), 100, 1), (1, GETDATE(), 200, 0)", connection);

            try
            {
                resetIdentityCusotmer.ExecuteNonQuery();
                resetIdentityOrder.ExecuteNonQuery();
                insertCustomerCommand.ExecuteNonQuery();
                insertOrderCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to insert test data: {exception}");
                throw;
            }
        }

        [Fact]
        public async Task Test_GetAllAsync()
        {

            var orders = await repository.GetAllAsync();
            Assert.NotNull(orders);
            Assert.Equal(2, orders.Count());
        }

        [Fact]
        public async Task Test_GetByIdAsync()
        {
            var order1 = await repository.GetByIdAsync(1);
            Assert.NotNull(order1);

            var order2 = await repository.GetByIdAsync(1001);
            Assert.Null(order2);
        }

        [Fact]
        public async Task Test_CreateAsync()
        {
            var initialOrders = await repository.GetAllAsync();
            Assert.Equal(2, initialOrders.Count());

            Order order = new Order(3, 1, DateTime.Now, 9999, false);
            var result = await repository.CreateAsync(order);
            Assert.NotNull(result);
            Assert.Equal(order, result);

            var finalOrders = await repository.GetAllAsync();
            Assert.Equal(3, finalOrders.Count());

            var insertedOrder = await repository.GetByIdAsync(3);

            Assert.Equal(order.ID, insertedOrder.ID);
            Assert.Equal(order.CustomerID, insertedOrder.CustomerID);
            Assert.Equal(order.TotalAmount, insertedOrder.TotalAmount);
            Assert.Equal(order.IsActive, insertedOrder.IsActive);

        }

        [Fact]
        public async Task Test_UpdateAsync()
        {
            var initialOrder = await repository.GetByIdAsync(1);

            Order orderToUpdate = new Order(initialOrder.ID, initialOrder.CustomerID, initialOrder.OrderDate, 0, !initialOrder.IsActive);
            await repository.UpdateAsync(orderToUpdate);

            var finalOrder = await repository.GetByIdAsync(1);
            Assert.NotNull(finalOrder);
            Assert.Equal(initialOrder.ID, finalOrder.ID);
            Assert.Equal(initialOrder.CustomerID, finalOrder.CustomerID);
            Assert.Equal(initialOrder.OrderDate, finalOrder.OrderDate);
            Assert.NotEqual(0, finalOrder.TotalAmount);
            Assert.NotEqual(!initialOrder.IsActive, finalOrder.IsActive);

        }

        [Fact]
        public async Task Test_DeleteAsync()
        {
            var initialOrder = await repository.GetByIdAsync(1);
            Assert.True(initialOrder.IsActive);

            await repository.DeleteAsync(1);

            var finalOrder = await repository.GetByIdAsync(1);
            Assert.NotNull(finalOrder);
            Assert.False(finalOrder.IsActive);

        }

        public void Dispose()
        {
            using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
            try
            {
                connection.Open();
                using SqlCommand deleteOrderCommand = new(
                    "DELETE FROM [Order];", connection);
                using SqlCommand deleteCustomerCommand = new(
                    "DELETE FROM Customer;", connection);
                deleteOrderCommand.ExecuteNonQuery();
                deleteCustomerCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to delete test data: {exception}");
                throw;
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

    }
}
