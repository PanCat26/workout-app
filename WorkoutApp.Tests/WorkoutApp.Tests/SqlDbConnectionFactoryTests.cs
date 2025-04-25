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
            var testConnStr = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            var factory = new SqlDbConnectionFactory(testConnStr);

            // Act
            var connection = factory.CreateConnection();

            // Assert
            Assert.NotNull(connection);
            Assert.IsType<SqlConnection>(connection);
            Assert.Equal(testConnStr, connection.ConnectionString);
        }

        [Fact]
        public async Task CreateConnection_OpensConnectionSuccessfully()
        {
            // Arrange
            var connStr = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            var factory = new SqlDbConnectionFactory(connStr);

            using var connection = factory.CreateConnection();

            // Act
            connection.Open();

            // Assert
            Assert.Equal(ConnectionState.Open, connection.State);
        }

    }
}
