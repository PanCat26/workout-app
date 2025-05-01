namespace WorkoutApp.Tests.Repository
{
    using Microsoft.Data.SqlClient;
    using System.Configuration;
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
            sessionManager.CurrentUserId = 1;
            repository = new OrderRepository(dbService, sessionManager);

            using var connection = (SqlConnection)connectionFactory.CreateConnection();
            connection.Open();

            using var insertCustomerCommand = new SqlCommand("INSERT INTO Customer (Name) VALUES ('TestUser')", connection);
            insertCustomerCommand.ExecuteNonQuery();

            connection.Close();
        }

        [Fact]
        public async Task Test_CreateAsync_CreatesOrderAndOrderItems()
        {
            using var connection = (SqlConnection)connectionFactory.CreateConnection();
            connection.Open();

            // Insert a category first (required for the foreign key in Product)
            using var insertCategory = new SqlCommand("INSERT INTO Category (Name) VALUES ('TestCategory'); SELECT SCOPE_IDENTITY();", connection);
            int categoryId = Convert.ToInt32(await insertCategory.ExecuteScalarAsync());

            // Insert a product linked to that category
            var product = new Product(
                id: 0,
                name: "Test Product",
                price: 49.99m,
                stock: 10,
                category: new Category(categoryId, "TestCategory"),
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
            insertProduct.Parameters.AddWithValue("@CategoryID", product.Category.ID);
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
            Assert.True(result.ID > 0);
        }

        public void Dispose()
        {
            using var connection = (SqlConnection)connectionFactory.CreateConnection();
            connection.Open();

            var cleanup = @"
                DELETE FROM OrderItem;
                DELETE FROM [Order];
                DELETE FROM Product;
                DELETE FROM Category;
                DELETE FROM Customer;
                DBCC CHECKIDENT ('OrderItem', RESEED, 0);
                DBCC CHECKIDENT ('[Order]', RESEED, 0);
                DBCC CHECKIDENT ('Product', RESEED, 0);
                DBCC CHECKIDENT ('Category', RESEED, 0);
                DBCC CHECKIDENT ('Customer', RESEED, 0);";

            using var cmd = new SqlCommand(cleanup, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            GC.SuppressFinalize(this);
        }
    }
}
