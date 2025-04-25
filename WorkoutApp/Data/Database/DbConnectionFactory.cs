// <copyright file="DbConnectionFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a factory for creating database connections.
    /// </summary>
    public abstract class DbConnectionFactory
    {
        /// <summary>
        /// The connection string used to establish database connections.
        /// </summary>
        protected readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionFactory"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to use for database connections.</param>
        protected DbConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Creates and returns a new database connection.
        /// </summary>
        /// <returns>An object that implements <see cref="IDbConnection"/>.</returns>
        public abstract IDbConnection CreateConnection();
    }
}
