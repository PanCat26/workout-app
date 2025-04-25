// <copyright file="TestDbConnectionFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Data.Database
{
    using System.Data;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// A factory for creating database connections for testing purposes.
    /// </summary>
    public class TestDbConnectionFactory : DbConnectionFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used for database connections.</param>
        public TestDbConnectionFactory(string connectionString)
        : base(connectionString)
        {
        }

        /// <summary>
        /// Creates and returns a new database connection using the provided connection string.
        /// </summary>
        /// <returns>A new instance of <see cref="SqlConnection"/>.</returns>
        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(this.connectionString);
        }
    }
}
