using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using WorkoutApp.Data.Database;
using System.Configuration;

namespace WorkoutApp.Tests
{
    public class SqlDbConnectionFactoryTests
    {
        [Fact]
        public void CreateConnection_ReturnsSqlConnection_WithCorrectConnectionString()
        {
            // Arrange
            var testConnectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            var factory = new SqlDbConnectionFactory(testConnectionString);

            // Act
            var connection = factory.CreateConnection();

            // Assert
            Assert.NotNull(connection);
            Assert.IsType<SqlConnection>(connection);
            Assert.Equal(testConnectionString, connection.ConnectionString);
        }

        [Fact]
        public async Task CreateConnection_OpensConnectionSuccessfully()
        {
            // Arrange
            var connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            var factory = new SqlDbConnectionFactory(connectionString);

            using var connection = factory.CreateConnection();

            // Act
            connection.Open();

            // Assert
            Assert.Equal(ConnectionState.Open, connection.State);
        }

    }
}
