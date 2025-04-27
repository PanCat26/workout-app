// <copyright file="DbService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// Represents the service used to interact with the database.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DbService"/> class with a <see cref="DbConnectionFactory"/> factory.
    /// </remarks>
    /// <param name="dbConnectionFactory">The database connection factory.</param>
    public class DbService(DbConnectionFactory dbConnectionFactory)
    {
        private readonly DbConnectionFactory dbConnectionFactory = dbConnectionFactory;

        /// <summary>
        /// Executes a Select statement.
        /// </summary>
        /// <param name="query">The SQL Select query to be run.</param>
        /// <param name="parameters">The parameters of the SQL Select query to be run.</param>
        /// <returns>A DataTable containing the results of the query.</returns>
        public DataTable ExecuteSelect(string query, List<SqlParameter> parameters)
        {
            using SqlConnection connection = (SqlConnection)this.dbConnectionFactory.CreateConnection();
            using SqlCommand command = new (query, connection);
            command.Parameters.AddRange([.. parameters]);
            using SqlDataAdapter adapter = new (command);
            DataTable dataTable = new ();

            try
            {
                connection.Open();
                adapter.Fill(dataTable);
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error executing query: {exception.Message}");
                throw;
            }

            return dataTable;
        }

        /// <summary>
        /// Executes an SQL query. Should be used for INSERT, UPDATE, DELETE statements.
        /// </summary>
        /// <param name="query">The SQL Select query to be run.</param>
        /// <param name="parameters">The parameters of the SQL Select query to be run.</param>
        /// <returns>Number of rows affected or -1 if an error occured.</returns>
        public int ExecuteQuery(string query, List<SqlParameter> parameters)
        {
            using SqlConnection connection = (SqlConnection)this.dbConnectionFactory.CreateConnection();
            using SqlCommand command = new (query, connection);
            command.Parameters.AddRange([.. parameters]);
            try
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error executing query: {exception.Message}");
                return -1;
            }
        }
    }
}
