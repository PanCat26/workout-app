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
            using SqlCommand insertCustomerCommand = new(
                "INSERT INTO Customer (ID, IsActive) VALUES (1, 1)", connection);
            using SqlCommand insertOrderCommand = new(
                "INSERT INTO [Order] (ID, CustomerId, OrderDate, TotalAmount, IsActive) VALUES (1, 1, GETDATE(), 100, 1), (2, 1, GETDATE(), 200, 0)", connection);
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
            Assert.Equal(1, (int)firstRow["ID"]);
            Assert.Equal(1, (int)firstRow["CustomerId"]);
            Assert.Equal(100m, Convert.ToDecimal(firstRow["TotalAmount"]));
            Assert.True(Convert.ToBoolean(firstRow["IsActive"]));
            DataRow secondRow = result.Rows[1];
            Assert.Equal(2, (int)secondRow["ID"]);
            Assert.Equal(1, (int)secondRow["CustomerId"]);
            Assert.Equal(200m, Convert.ToDecimal(secondRow["TotalAmount"]));
            Assert.False(Convert.ToBoolean(secondRow["IsActive"]));
        }

        [Fact]
        public async Task ExecuteSelect_ValidQueryWithParameters_ReturnsDataTable()
        {
            // Arrange
            string query = "SELECT * FROM [Order] WHERE ID = @Id";
            List<SqlParameter> parameters =
            [
                new SqlParameter("@ID", 1)
            ];
            // Act
            DataTable result = await dbService.ExecuteSelectAsync(query, parameters);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Rows.Count);
            DataRow row = result.Rows[0];
            Assert.Equal(1, (int)row["ID"]);
            Assert.Equal(1, (int)row["CustomerId"]);
            Assert.Equal(100m, Convert.ToDecimal(row["TotalAmount"]));
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
            string query = "DELETE FROM [Order] WHERE ID = @Id";
            List<SqlParameter> parameters =
            [
                new SqlParameter("@ID", 1)
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
