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
    /// <remarks>
    /// Initializes a new instance of the <see cref="DbConnectionFactory"/> class with the specified connection string.
    /// </remarks>
    /// <param name="connectionString">The connection string to use for the SQL database connection.</param>
    public class DbConnectionFactory(string connectionString)
    {
        private readonly string connectionString = connectionString;

        /// <summary>
        /// Creates and returns a new SQL database connection.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> instance representing the SQL database connection.</returns>
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(this.connectionString);
        }
    }
}
