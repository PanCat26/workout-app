/*using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using WorkoutApp.Data.Database;
using WorkoutApp.Models;
using WorkoutApp.Repository;

namespace WorkoutApp.Tests.Repository
{
    [Collection("DatabaseTests")]
    public class OrderDetailRepositoryTests : IDisposable
    {
        // all tests are running on the assumption that all the tables are empty
        private readonly OrderDetailRepository repository;
        private readonly DbConnectionFactory connectionFactory;
        private readonly DbService dbService;

        public OrderDetailRepositoryTests()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            connectionFactory = new SqlDbConnectionFactory(connectionString);
            dbService = new DbService(connectionFactory);
            repository = new OrderDetailRepository(dbService);

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


            using SqlCommand insertCategoryCommand = new("INSERT INTO Category (Name, IsActive) VALUES ('Fitness Equipment', 1)", connection);
            using SqlCommand insertProductCommand = new("INSERT INTO Product (Name, Price, Stock, CategoryID, Atributes, Size, Description, FileUrl, IsActive) " +
                "VALUES ('Yoga Mat', 29.99, 100, 1, 'Blue', 'Standard', 'High quality yoga mat', 'url_to_image.jpg', 1)," +
                "('Whey Protein', 49.99, 50, 1, NULL, '2kg', 'Premium whey protein', 'url_to_protein.jpg', 1)", connection);
            using SqlCommand insertCustomerCommand = new(
               "INSERT INTO Customer (IsActive) VALUES (1)", connection);
            using SqlCommand insertOrderCommand = new(
                "INSERT INTO [Order] (CustomerId, OrderDate, TotalAmount, IsActive) VALUES (1, GETDATE(), 100, 1), (1, GETDATE(), 200, 0)", connection);
            using SqlCommand insertOrderDetailCommand = new("INSERT INTO OrderDetail (OrderID, ProductID, Quantity, Price, IsActive) VALUES (1, 1, 2, 29.99, 1), (1, 2, 1, 49.99, 1)", connection);

            try
            {

                insertCategoryCommand.ExecuteNonQuery();
                insertProductCommand.ExecuteNonQuery();
                insertCustomerCommand.ExecuteNonQuery();
                insertOrderCommand.ExecuteNonQuery();
                insertOrderDetailCommand.ExecuteNonQuery();
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
            var orders = await repository.GetAllAsync();
            Assert.NotNull(orders);
            Assert.Equal(2, orders.Count());
        }

        [Fact]
        public async Task Test_GetByIdAsync()
        {
            var order1 = await repository.GetByIdAsync(1);
            Assert.NotNull(order1);
            Assert.Equal(1, order1.ID);

            var order2 = await repository.GetByIdAsync(1001);
            Assert.Null(order2);
        }

        [Fact]
        public async Task Test_CreateAsync()
        {
            var initialOrders = await repository.GetAllAsync();
            Assert.NotNull(initialOrders);
            Assert.Equal(2, initialOrders.Count());

            OrderDetail order = new OrderDetail(3, 1, 2, 100, 999, true);
            var result = await repository.CreateAsync(order);
            Assert.NotNull(result);

            var finalOrders = await repository.GetAllAsync();
            Assert.NotNull(finalOrders);
            Assert.Equal(3, finalOrders.Count());

            var insertedOrder = await repository.GetByIdAsync(3);
            Assert.NotNull(insertedOrder);

            Assert.Equal(order.ID, insertedOrder.ID);
            Assert.Equal(order.OrderID, insertedOrder.OrderID);
            Assert.Equal(order.ProductID, insertedOrder.ProductID);
            Assert.Equal(order.Price, insertedOrder.Price);
            Assert.Equal(order.Quantity, insertedOrder.Quantity);
            Assert.Equal(order.IsActive, insertedOrder.IsActive);

        }

        [Fact]
        public async Task Test_UpdateAsync()
        {
            var initialOrder = await repository.GetByIdAsync(1);
            Assert.NotNull(initialOrder);

            OrderDetail orderToUpdate = new OrderDetail(initialOrder.ID, initialOrder.OrderID, initialOrder.ProductID, 0, 0, !initialOrder.IsActive);
            await repository.UpdateAsync(orderToUpdate);

            var finalOrder = await repository.GetByIdAsync(1);
            Assert.NotNull(finalOrder);
            Assert.Equal(initialOrder.ID, finalOrder.ID);
            Assert.Equal(initialOrder.OrderID, finalOrder.OrderID);
            Assert.Equal(initialOrder.ProductID, finalOrder.ProductID);
            Assert.Equal(0, finalOrder.Price);
            Assert.Equal(0, finalOrder.Quantity);
            Assert.Equal(!initialOrder.IsActive, finalOrder.IsActive);
        }

        [Fact]
        public async Task Test_DeleteAsync()
        {
            var initialOrder = await repository.GetByIdAsync(1);
            Assert.NotNull(initialOrder);
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
                using SqlCommand deleteOrderDetailCommand = new("DELETE FROM OrderDetail", connection);
                using SqlCommand deleteOrderCommand = new("DELETE FROM [Order]", connection);
                using SqlCommand deleteCustomerCommand = new("DELETE FROM Customer", connection);
                using SqlCommand deleteProductCommand = new("DELETE FROM Product", connection);
                using SqlCommand deleteCategoryCommand = new("DELETE FROM Category", connection);

                deleteOrderDetailCommand.ExecuteNonQuery();
                deleteOrderCommand.ExecuteNonQuery();
                deleteCustomerCommand.ExecuteNonQuery();
                deleteProductCommand.ExecuteNonQuery();
                deleteCategoryCommand.ExecuteNonQuery();

                // Use square brackets for all table names for consistency and correctness
                using SqlCommand resetIdentityCategory = new("DBCC CHECKIDENT ('[Category]', RESEED, 0)", connection);
                using SqlCommand resetIdentityProduct = new("DBCC CHECKIDENT ('[Product]', RESEED, 0)", connection);
                using SqlCommand resetIdentityCustomer = new("DBCC CHECKIDENT ('[Customer]', RESEED, 0)", connection);
                using SqlCommand resetIdentityOrder = new("DBCC CHECKIDENT ('[Order]', RESEED, 0)", connection);
                using SqlCommand resetIdentityOrderDetail = new("DBCC CHECKIDENT ('[OrderDetail]', RESEED, 0)", connection);

                resetIdentityCategory.ExecuteNonQuery();
                resetIdentityProduct.ExecuteNonQuery();
                resetIdentityCustomer.ExecuteNonQuery();
                resetIdentityOrder.ExecuteNonQuery();
                resetIdentityOrderDetail.ExecuteNonQuery();
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
*/