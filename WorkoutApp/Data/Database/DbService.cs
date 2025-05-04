// <copyright file="DbService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
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
        public virtual async Task<DataTable> ExecuteSelectAsync(string query, List<SqlParameter> parameters)
        {
            using SqlConnection connection = (SqlConnection)this.dbConnectionFactory.CreateConnection();
            using SqlCommand command = new (query, connection);
            command.Parameters.AddRange([.. parameters]);
            using SqlDataAdapter adapter = new (command);
            DataTable dataTable = new ();

            try
            {
                await connection.OpenAsync();
                using SqlDataReader reader = await command.ExecuteReaderAsync();
                dataTable.Load(reader);
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
        public async Task<int> ExecuteQueryAsync(string query, List<SqlParameter> parameters)
        {
            using SqlConnection connection = (SqlConnection)this.dbConnectionFactory.CreateConnection();
            using SqlCommand command = new (query, connection);
            command.Parameters.AddRange([.. parameters]);
            try
            {
                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error executing query: {exception.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Executes an SQL query and returns the first column of the first row in the result set returned by the query.
        /// </summary>
        /// <typeparam name="T">Type of object expected to be returned.</typeparam>
        /// <param name="query">The SQL Select query to be run.</param>
        /// <param name="parameters">The parameters of the SQL Select query to be run.</param>
        /// <returns>A <see cref="Task{T}"/> representing the result of the asynchronous operation.</returns>
        public async Task<T?> ExecuteScalarAsync<T>(string query, List<SqlParameter> parameters)
        {
            using SqlConnection connection = (SqlConnection)this.dbConnectionFactory.CreateConnection();
            using SqlCommand command = new (query, connection);
            command.Parameters.AddRange([.. parameters]);

            try
            {
                await connection.OpenAsync();
                object? result = await command.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                {
                    throw new InvalidOperationException("Result is null or DBNull.");
                }

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error executing query: {exception.Message}");
                throw;
            }
        }
    }
}
