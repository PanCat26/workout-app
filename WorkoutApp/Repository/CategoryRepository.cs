// <copyright file="CategoryRepository.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
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
    public class CategoryRepository : IRepository<Category> // Implementing the IRepository interface
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

        /// <summary>
        /// Retrieves all categories asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a collection of categories.</returns>
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            // Select all categories. No IsActive column in schema, so select all.
            var query = "SELECT ID, Name FROM Category";
            // Use DbService to execute the select query. No parameters needed.
            DataTable dataTable = await this.dbService.ExecuteSelectAsync(query, new List<SqlParameter>());

            var categories = new List<Category>();

            // Iterate through the rows returned by the database
            foreach (DataRow row in dataTable.Rows)
            {
                // Map DataRow to Category object
                var category = new Category(

                    // ID is INT PRIMARY KEY IDENTITY, retrieve the DB generated ID

                    id: Convert.ToInt32(row["ID"]),
                    // Name is NVARCHAR(100) NOT NULL
                    name: row["Name"].ToString()
                );
                // Note: No IsActive property mapping as per schema

                categories.Add(category);
            }

            return categories;
        }

        /// <summary>
        /// Retrieves a category by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the category to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the category if found.</returns>
        public async Task<Category?> GetByIdAsync(int id) // Corrected: Using int id as per IRepository
        {
            // Define parameters for the query
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id) // Using int id
            };

            // Select a category by ID. No IsActive column in schema.
            var query = "SELECT ID, Name FROM Category WHERE ID = @ID";

            // Use DbService to execute the select query
            DataTable dataTable = await this.dbService.ExecuteSelectAsync(query, parameters);

            // If no rows are returned, the category was not found
            if (dataTable.Rows.Count == 0)
            {
                return null;
            }

            // Get the first row (should be only one since ID is primary key)
            DataRow row = dataTable.Rows[0];

            // Map DataRow to Category object
            var category = new Category(
                 id: Convert.ToInt32(row["ID"]),
                 name: row["Name"].ToString()
            );
            // Note: No IsActive property mapping as per schema

            return category;
        }

        /// <summary>
        /// Creates a new category in the database asynchronously.
        /// Assumes Category.ID IS an IDENTITY column.
        /// The ID from the input entity is ignored as the database generates it.
        /// </summary>
        /// <param name="entity">The category to create.</param>
        /// <returns>A task representing the asynchronous operation, containing the created category with the database-assigned ID.</returns>
        public async Task<Category> CreateAsync(Category entity)
        {
            // Define parameters for the insert query.
            // Do NOT include the ID parameter as it's an IDENTITY column managed by the DB.
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", entity.Name)
                // No IsActive parameter as per schema
            };

            // Insert a new row and use OUTPUT INSERTED.ID to get the database-generated ID.
            // Do NOT include the ID column in the INSERT column list.
            string query = @"
            INSERT INTO Category (Name)
            OUTPUT INSERTED.ID
            VALUES (@Name);";

            // Execute the insert query using DbService.
            // ExecuteSelectAsync is used because OUTPUT INSERTED.ID returns a result set.
            DataTable resultTable = await this.dbService.ExecuteSelectAsync(query, parameters);

            // Retrieve the generated ID from the result set
            int newId = Convert.ToInt32(resultTable.Rows[0][0]);

            // Update the input entity's ID with the generated ID
            entity.ID = newId;

            return entity; // Return the entity with the assigned ID
        }


        /// <summary>
        /// Updates an existing category asynchronously.
        /// </summary>
        /// <param name="entity">The category to update. Must have a valid ID.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated category.</returns>
        public async Task<Category> UpdateAsync(Category entity)
        {
            // Ensure a valid ID is provided for the update
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", entity.ID),
                new SqlParameter("@Name", entity.Name)
                // No IsActive parameter as per schema
            };

            // Update the Name column for the specified ID. No IsActive column in schema.
            var query = "UPDATE Category SET Name = @Name WHERE ID = @ID";

            // Use DbService to execute the update query
            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);

            // Optional: Log a warning if no rows were updated (category not found)
            if (affectedRows == 0)
            {
                Debug.WriteLine($"Warning: UpdateAsync for Category ID {entity.ID} affected 0 rows.");
            }

            return entity; // Return the entity after attempting the update
        }

        /// <summary>
        /// Deletes a category by its identifier asynchronously.
        /// Based on schema and IRepository, this is a hard delete.
        /// If soft delete is needed, add an IsActive column to the DB schema.
        /// </summary>
        /// <param name="id">The identifier of the category to delete.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating success.</returns>
        public async Task<bool> DeleteAsync(int id) // Corrected: Using int id as per IRepository
        {
            // Define parameters for the delete query
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ID", id) // Using int id
            };

            // Perform a hard delete as there's no IsActive column in the schema for soft delete
            var query = "DELETE FROM Category WHERE ID = @ID";

            // Use DbService to execute the delete query
            int affectedRows = await this.dbService.ExecuteQueryAsync(query, parameters);

            // Return true if at least one row was affected (category was found and deleted)
            return affectedRows > 0;
        }

    }
}
