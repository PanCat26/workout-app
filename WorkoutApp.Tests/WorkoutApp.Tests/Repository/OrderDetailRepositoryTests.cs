using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using WorkoutApp.Data.Database;
using WorkoutApp.Repository;

namespace WorkoutApp.Tests.Repository
{
    class OrderDetailRepositoryTests : IDisposable
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


            using SqlCommand resetIdentityCategory = new("DBCC CHECKIDENT ('Category', RESEED, 0)", connection);
            using SqlCommand resetIdentityProduct = new("DBCC CHECKIDENT ('Product', RESEED, 0)", connection);
            using SqlCommand resetIdentityCusotmer = new("DBCC CHECKIDENT ('Customer', RESEED, 0)", connection);
            using SqlCommand resetIdentityOrder = new("DBCC CHECKIDENT ('Order', RESEED, 0)", connection);
            using SqlCommand resetIdentityOrderDetail = new("DBCC CHECKIDENT ('OrderDetail', RESEED, 0)", connection);

            using SqlCommand insertCategoryCommand = new("INSERT INTO Category (Name, IsActive) VALUES ('Fitness Equipment', 1)", connection);
            using SqlCommand insertProductCommand = new("INSERT INTO Product (Name, Price, Stock, CategoryID, Atributes, Size, Description, FileUrl, IsActive) " +
                "VALUES ('Yoga Mat', 29.99, 100, 1, 'Blue', 'Standard', 'High quality yoga mat', 'url_to_image.jpg', 1)," +
                "('Whey Protein', 49.99, 50, 2, NULL, '2kg', 'Premium whey protein', 'url_to_protein.jpg', 1)", connection);
            using SqlCommand insertCustomerCommand = new(
               "INSERT INTO Customer (IsActive) VALUES (1)", connection);
            using SqlCommand insertOrderCommand = new(
                "INSERT INTO [Order] (CustomerId, OrderDate, TotalAmount, IsActive) VALUES (1, GETDATE(), 100, 1), (1, GETDATE(), 200, 0)", connection);
            using SqlCommand insertOrderDetailCommand = new("INSERT INTO CartItem (CartID, ProductID, Quantity, IsActive) VALUES (1, 1, 2, 1), (1, 2, 1, 1)", connection);

            try
            {
                resetIdentityCategory.ExecuteNonQuery();
                resetIdentityProduct.ExecuteNonQuery();
                resetIdentityCusotmer.ExecuteNonQuery();
                resetIdentityOrder.ExecuteNonQuery();
                resetIdentityOrderDetail.ExecuteNonQuery();

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
        }

        public void Dispose()
        {

        }

    }
}
