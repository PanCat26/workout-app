// <copyright file="CategoryRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data; // Required for DataTable and DataRow
    using System.Diagnostics; // Required for Debug.WriteLine (optional, but good for errors)
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient; // Required for SqlParameter
    using WorkoutApp.Data.Database; // Assuming DbService is here
    using WorkoutApp.Models; // Assuming Category model is here

    /// <summary>
    /// Repository class for managing Category items in the database.
    /// Implements the <see cref="IRepository{Category}"/> interface.
    /// </summary>
    public class CategoryRepository : IRepository<Category>
    {
        private readonly DbService dbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="dbService">The database service used for executing queries.</param>
        public CategoryRepository(DbService dbService)
        {
            this.dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        /// <returns>A collection of <see cref="Category"/> objects.</returns>
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var query = "SELECT ID, Name FROM Category";
            DataTable dataTable = await this.dbService.ExecuteSelectAsync(query, new List<SqlParameter>());

            var categories = new List<Category>();

            foreach (DataRow row in dataTable.Rows)
            {
                var category = new Category(
                    id: Convert.ToInt32(row["ID"]),
                    name: row["Name"].ToString() ?? string.Empty);

                categories.Add(category);
            }

            return categories;
        }

        /// <summary>
        /// Retrieves a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to retrieve.</param>
        /// <returns>A <see cref="Category"/> object if found; otherwise, <c>null</c>.</returns>
        public async Task<Category?> GetByIdAsync(int id)
        {
            var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@ID", id),
                    };

            var query = "SELECT ID, Name FROM Category WHERE ID = @ID";
            DataTable dataTable = await this.dbService.ExecuteSelectAsync(query, parameters);

            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dataTable.Rows[0];

            var category = new Category(
                id: Convert.ToInt32(row["ID"]),
                name: row["Name"].ToString() ?? string.Empty);

            return category;
        }

        /// <inheritdoc/>
        public async Task<Category> CreateAsync(Category entity)
        {
            var parameters = new List<SqlParameter>
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
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", entity.ID),
                new SqlParameter("@Name", entity.Name),
            };

            var query = "UPDATE Category SET Name = @Name WHERE ID = @ID";
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
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id),
            };

            var query = "DELETE FROM Category WHERE ID = @ID";
            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);

            return affectedRows > 0;
        }
    }
}
