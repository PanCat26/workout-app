namespace WorkoutApp.Tests.Repository
{
    using Microsoft.Data.SqlClient;
    using System.Configuration;
    using System.Diagnostics;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Infrastructure.Session;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;

    [Collection("DatabaseTests")]
    public class OrderRepositoryTests : IDisposable
    {
        private readonly OrderRepository repository;
        private readonly DbConnectionFactory connectionFactory;
        private readonly DbService dbService;
        private readonly SessionManager sessionManager;

        public OrderRepositoryTests()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            connectionFactory = new DbConnectionFactory(connectionString);
            dbService = new DbService(connectionFactory);
            sessionManager = new SessionManager();
            repository = new OrderRepository(dbService, sessionManager);

            using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
            try
            {
                connection.Open();

                // Insert one customer
                using var insertCustomer = new SqlCommand("INSERT INTO Customer (Name) VALUES ('TestCusotmer');", connection);
                insertCustomer.ExecuteNonQuery();

                // Insert one category (required by product FK)
                using var insertCategory = new SqlCommand("INSERT INTO Category (Name) VALUES ('TestCategory'); SELECT SCOPE_IDENTITY();", connection);
                int categoryId = Convert.ToInt32(insertCategory.ExecuteScalar());
                Debug.WriteLine($"Inserted category with ID: {categoryId}");

                // Insert one product using that category
                using var insertProduct = new SqlCommand(@"
                    INSERT INTO Product (Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL)
                    VALUES ('Test Product', 49.99, 100, @CategoryID, 'M', 'Red', 'Sample description', 'http://example.com/photo.jpg');", connection);
                insertProduct.Parameters.AddWithValue("@CategoryID", categoryId);
                insertProduct.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to insert mock data: {exception}");
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task Test_CreateAsync_CreatesOrderAndOrderItems()
        {

            using var connection = (SqlConnection)connectionFactory.CreateConnection();
            connection.Open();

            // Insert a product linked to that category
            var product = new Product(
                id: 0,
                name: "Test Product",
                price: 49.99m,
                stock: 10,
                category: new Category(1, "TestCategory"),
                size: "M",
                color: "Red",
                description: "Test Description",
                photoURL: "http://example.com/photo.jpg");

            using var insertProduct = new SqlCommand(@"
                INSERT INTO Product (Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL)
                VALUES (@Name, @Price, @Stock, @CategoryID, @Size, @Color, @Description, @PhotoURL);
                SELECT SCOPE_IDENTITY();", connection);

            insertProduct.Parameters.AddWithValue("@Name", product.Name);
            insertProduct.Parameters.AddWithValue("@Price", product.Price);
            insertProduct.Parameters.AddWithValue("@Stock", product.Stock);
            insertProduct.Parameters.AddWithValue("@CategoryID", 1);
            insertProduct.Parameters.AddWithValue("@Size", product.Size);
            insertProduct.Parameters.AddWithValue("@Color", product.Color);
            insertProduct.Parameters.AddWithValue("@Description", product.Description);
            insertProduct.Parameters.AddWithValue("@PhotoURL", product.PhotoURL);

            product.ID = Convert.ToInt32(await insertProduct.ExecuteScalarAsync());

            connection.Close();


            var order = new Order(0, new List<OrderItem>
            {
                new OrderItem(product, 2),
            }, DateTime.Now);

            var result = await repository.CreateAsync(order);

            Assert.NotNull(result);
            Debug.WriteLine($"Order ID: {result.ID}");
            Assert.True(result.ID == 1);
        }
        [Fact]
        public async Task Test_GetAllAsync_ReturnsEmptyList()
        {
            var orders = await repository.GetAllAsync();
            Assert.NotNull(orders);
            Assert.Empty(orders);
        }

        [Fact]
        public async Task Test_GetByIdAsync_ReturnsOrderWithGivenId()
        {
            var order = await repository.GetByIdAsync(0);

            Assert.NotNull(order);
            Assert.Empty(order.OrderItems);
        }

        [Fact]
        public async Task Test_UpdateAsync_ReturnsNewOrderInstance()
        {
            var updatedOrder = new Order(1, new List<OrderItem>(), DateTime.Now);

            var result = await repository.UpdateAsync(updatedOrder);

            Assert.NotNull(result);
            Assert.Equal(0, result.ID); // per stubbed method
            Assert.Empty(result.OrderItems);
        }

        [Fact]
        public async Task Test_DeleteAsync_ReturnsTrue()
        {
            var result = await repository.DeleteAsync(1);

            Assert.True(result);
        }


        public void Dispose()
        {
            using var connection = (SqlConnection)connectionFactory.CreateConnection();
            connection.Open();

            var cleanup = @"
        -- Ensure proper delete order due to FKs
        DELETE FROM OrderItem;
        DELETE FROM [Order];
        DELETE FROM Product;
        DELETE FROM Category;
        DELETE FROM Customer;";


            using var cmd = new SqlCommand(cleanup, connection);
            cmd.ExecuteNonQuery();

            var cleanup2 = @"-- Reset identity columns
        DBCC CHECKIDENT ('OrderItem', RESEED, 0);
        DBCC CHECKIDENT ('[Order]', RESEED, 0);
        DBCC CHECKIDENT ('Product', RESEED, 0);
        DBCC CHECKIDENT ('Category', RESEED, 0);
        DBCC CHECKIDENT ('Customer', RESEED, 0);";

            using var cmd2 = new SqlCommand(cleanup2, connection);
            cmd2.ExecuteNonQuery();
            connection.Close();

            GC.SuppressFinalize(this);
        }

    }
}
