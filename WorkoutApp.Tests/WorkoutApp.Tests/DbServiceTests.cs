using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using WorkoutApp.Data.Database;

namespace WorkoutApp.Tests
{
    [Collection("DatabaseTests")]
    public class DbServiceTests : IDisposable
    {
        private readonly DbConnectionFactory connectionFactory;
        private readonly DbService dbService;

        public DbServiceTests()
        {
            string? testConnectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(testConnectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            connectionFactory = new DbConnectionFactory(testConnectionString);
            dbService = new DbService(connectionFactory);

            try
            {
                using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
                connection.Open();

                // Reset ids in test database
                using var resetIdsCommand = new SqlCommand(
                    "DBCC CHECKIDENT ('Category', RESEED, 0);",
                    connection
                );
                resetIdsCommand.ExecuteNonQuery();

                using var insertCategoryCommand = new SqlCommand(
                    "INSERT INTO Category (Name) VALUES ('Creatine'), ('Pants');",
                    connection
                );
                insertCategoryCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database setup failed: {ex}");
                throw;
            }
        }

        [Fact]
        public async Task ExecuteSelect_ValidQueryNoParameters_ReturnsDataTable()
        {
            // Arrange
            string query = "SELECT * FROM Category";
            // Act
            DataTable result = await dbService.ExecuteSelectAsync(query, []);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Rows.Count);
            DataRow firstRow = result.Rows[0];
            Assert.Equal("Creatine", firstRow["Name"]);
            DataRow secondRow = result.Rows[1];
            Assert.Equal("Pants", secondRow["Name"]);
        }

        [Fact]
        public async Task ExecuteSelect_ValidQueryWithParameters_ReturnsDataTable()
        {
            // Arrange
            string query = "SELECT * FROM Category WHERE Name = @Name";
            List<SqlParameter> parameters =
            [
                new SqlParameter("@Name", "Creatine")
            ];
            // Act
            DataTable result = await dbService.ExecuteSelectAsync(query, parameters);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Rows.Count);
            DataRow row = result.Rows[0];
            Assert.Equal("Creatine", row["Name"]);
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
            string query = "DELETE FROM Category;";
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
            string query = "DELETE FROM Category WHERE Name = @Name";
            List<SqlParameter> parameters =
            [
                new SqlParameter("@Name", "Creatine")
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

        [Fact]
        public async Task ExecuteScalarAsync_InvalidQuery_ThrowsInvalidOperationException()
        {
            // Arrange
            string query = "SELECT * FROM Category WHERE Category.ID=-1;";
            List<SqlParameter> parameters = [];
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await dbService.ExecuteScalarAsync<int>(query, parameters));
        }

        public void Dispose()
        {
            try
            {
                using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
                connection.Open();
                using SqlCommand deleteCategoryCommand = new(
                    "DELETE FROM Category;", connection);
                deleteCategoryCommand.ExecuteNonQuery();
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
