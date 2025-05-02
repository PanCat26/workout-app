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
    public class DbConnectionFactoryTests
    {
        [Fact]
        public void CreateConnection_ReturnsSqlConnection_WithCorrectConnectionString()
        {
            // Arrange
            string? testConnectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(testConnectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            DbConnectionFactory factory = new(testConnectionString);

            // Act
            using SqlConnection connection = (SqlConnection)factory.CreateConnection();

            // Assert
            Assert.NotNull(connection);
            Assert.IsType<SqlConnection>(connection);
            Assert.Equal(testConnectionString, connection.ConnectionString);
        }

        [Fact]
        public void CreateConnection_OpensConnectionSuccessfully()
        {
            // Arrange
            string? testConnectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(testConnectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            DbConnectionFactory factory = new(testConnectionString);

            using SqlConnection connection = (SqlConnection)factory.CreateConnection();

            // Act
            connection.Open();

            // Assert
            Assert.Equal(ConnectionState.Open, connection.State);
        }

    }
}
