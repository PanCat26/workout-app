using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Data.Database;

namespace WorkoutApp.Tests
{
    public class DbServiceTests : IDisposable
    {
        private readonly DbConnectionFactory connectionFactory;
        private readonly DbService dbService;

        public DbServiceTests()
        {
            string? connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            connectionFactory = new TestDbConnectionFactory(connectionString);
            dbService = new DbService(connectionFactory);

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

            try
            {
                using var insertCustomerCommand = new SqlCommand(
                    "INSERT INTO Customer (IsActive) OUTPUT INSERTED.Id VALUES (1);",
                    connection
                );

                int insertedCustomerId = (int)insertCustomerCommand.ExecuteScalar();

                using var insertOrderCommand = new SqlCommand(
                    @"INSERT INTO [Order] (CustomerId, OrderDate, TotalAmount, IsActive)
                      VALUES 
                      (@CustomerId, GETDATE(), 100, 1),
                      (@CustomerId, GETDATE(), 200, 0);",
                    connection
                );

                insertOrderCommand.Parameters.AddWithValue("@CustomerId", insertedCustomerId);
                insertOrderCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to insert test data: {ex}");
                throw;
            }
        }
        
        [Fact]
        public async Task ExecuteSelect_ValidQueryNoParameters_ReturnsDataTable()
        {
            // Arrange
            string query = "SELECT * FROM [Order]";
            // Act
            DataTable result = await dbService.ExecuteSelectAsync(query, []);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Rows.Count);
            DataRow firstRow = result.Rows[0];
            Assert.Equal(100.0, firstRow["TotalAmount"]);
            Assert.True(Convert.ToBoolean(firstRow["IsActive"]));
            DataRow secondRow = result.Rows[1];
            Assert.Equal(200.0, secondRow["TotalAmount"]);
            Assert.False(Convert.ToBoolean(secondRow["IsActive"]));
        }

        [Fact]
        public async Task ExecuteSelect_ValidQueryWithParameters_ReturnsDataTable()
        {
            // Arrange
            string query = "SELECT * FROM [Order] WHERE TotalAmount = @TotalAmount";
            List<SqlParameter> parameters =
            [
                new SqlParameter("@TotalAmount", 100.0)
            ];
            // Act
            DataTable result = await dbService.ExecuteSelectAsync(query, parameters);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Rows.Count);
            DataRow row = result.Rows[0];
            Assert.Equal(100.0, row["TotalAmount"]);
            Assert.True(Convert.ToBoolean(row["IsActive"]));
        }

        [Fact]
        public async Task ExecuteSelect_InvalidQuery_ThrowsSqlException()
        {
            // Arrange
            string query = "SELECT * FROM NonExistentTable";
            // Act & Assert
            await Assert.ThrowsAsync<SqlException>(async () => await dbService.ExecuteSelectAsync(query, []));
        }

        [Fact]
        public async Task ExecuteQuery_ValidQueryNoParameters_ReturnsAffectedRows()
        {
            // Arrange
            string query = "DELETE FROM [Order];";
            List<SqlParameter> parameters = [];
            // Act
            int affectedRows = await dbService.ExecuteQueryAsync(query, parameters);
            // Assert
            Assert.Equal(2, affectedRows);
        }

        [Fact]
        public async Task ExecuteQuery_ValidQueryWithParameters_ReturnsAffectedRows()
        {
            // Arrange
            string query = "DELETE FROM [Order] WHERE TotalAmount = @TotalAmount";
            List<SqlParameter> parameters =
            [
                new SqlParameter("@TotalAmount", 100.0)
            ];
            // Act
            int affectedRows = await dbService.ExecuteQueryAsync(query, parameters);
            // Assert
            Assert.Equal(1, affectedRows);
        }

        [Fact]
        public async Task ExecuteQuery_InvalidQuery_ReturnsNegativeValue()
        {
            // Arrange
            string query = "DELETE FROM NonExistentTable";
            List<SqlParameter> parameters = [];
            // Act
            int affectedRows = await dbService.ExecuteQueryAsync(query, parameters);
            // Assert
            Assert.Equal(-1, affectedRows);
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
