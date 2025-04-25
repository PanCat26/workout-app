// <copyright file="SqlDbConnectionFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Data.Database
{
    using System.Data;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// A factory class for creating SQL database connections.
    /// </summary>
    public class SqlDbConnectionFactory : DbConnectionFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbConnectionFactory"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to use for the SQL database connection.</param>
        public SqlDbConnectionFactory(string connectionString)
        : base(connectionString)
        {
        }

        /// <summary>
        /// Creates and returns a new SQL database connection.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> instance representing the SQL database connection.</returns>
        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(this.connectionString);
        }
    }
}
