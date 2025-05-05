// <copyright file="CategoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Models;

    /// <summary>
    /// Repository class for managing Category items in the database.
    /// </summary>
    public class CategoryRepository : IRepository<Category>
    {
        private readonly DbService dbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service.</param>
        public CategoryRepository(DbService dbService)
        {
            this.dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            string query = "SELECT ID, Name FROM Category";
            DataTable dataTable = await this.dbService.ExecuteSelectAsync(query, new List<SqlParameter>());

            List<Category> categories = new ();

            foreach (DataRow row in dataTable.Rows)
            {
                Category category = new (
                    id: Convert.ToInt32(row["ID"]),
                    name: row["Name"].ToString() ?? string.Empty);
                categories.Add(category);
            }

            return categories;
        }

        /// <inheritdoc/>
        public async Task<Category?> GetByIdAsync(int id)
        {
            List<SqlParameter> parameters = new ()
            {
                new SqlParameter("@ID", id),
            };

            string query = "SELECT ID, Name FROM Category WHERE ID = @ID";
            DataTable dataTable = await this.dbService.ExecuteSelectAsync(query, parameters);

            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dataTable.Rows[0];
            Category category = new (
                id: Convert.ToInt32(row["ID"]),
                name: row["Name"].ToString() ?? string.Empty);

            return category;
        }

        /// <inheritdoc/>
        public async Task<Category> CreateAsync(Category entity)
        {
            List<SqlParameter> parameters = new ()
            {
                new SqlParameter("@Name", entity.Name),
            };

            string query = @"
                INSERT INTO Category (Name)
                OUTPUT INSERTED.ID
                VALUES (@Name);";

            DataTable resultTable = await this.dbService.ExecuteSelectAsync(query, parameters);
            int newId = Convert.ToInt32(resultTable.Rows[0][0]);
            entity.ID = newId;

            return entity;
        }

        /// <inheritdoc/>
        public async Task<Category> UpdateAsync(Category entity)
        {
            List<SqlParameter> parameters = new ()
            {
                new SqlParameter("@ID", entity.ID),
                new SqlParameter("@Name", entity.Name),
            };

            string query = "UPDATE Category SET Name = @Name WHERE ID = @ID";
            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);

            if (affectedRows == 0)
            {
                Debug.WriteLine($"Warning: UpdateAsync for Category ID {entity.ID} affected 0 rows.");
            }

            return entity;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            List<SqlParameter> parameters = new ()
            {
                new SqlParameter("@ID", id),
            };

            string query = "DELETE FROM Category WHERE ID = @ID";
            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);

            return affectedRows > 0;
        }
    }
}
