using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration; // Required for ConfigurationManager
using System.Data; // Required for DataTable, DataRow
using System.Diagnostics; // Required for Debug.WriteLine
using System.Linq;
using System.Threading.Tasks;
using WorkoutApp.Data.Database; // Assuming DbService and DbConnectionFactory are here
using WorkoutApp.Models; // Assuming Category model is here
using WorkoutApp.Repository; // Assuming CategoryRepository and IRepository are here
using WorkoutApp.Service; // Assuming CategoryService and IService are here
using Xunit; // Required for [Fact], Assert, [Collection], IDisposable

// Add this attribute to disable parallel execution for this test class.
// This is CRUCIAL for preventing deadlocks and ensuring a clean database state
// between tests that share the same database.
// You need to define the 'Sequential' collection in a separate file (e.g., CollectionDefinitions.cs).
[Collection("DatabaseTests")]
public class CategoryServiceTests : IDisposable // Implement IDisposable for cleanup
{
    // These fields are used by the test class and service/repository dependencies
    private readonly DbConnectionFactory connectionFactory;
    private readonly DbService dbService; // Assuming DbService uses connectionFactory internally
    private readonly IRepository<Category> categoryRepository; // The repository dependency for the service
    private readonly CategoryService categoryService; // The service under test

    /// <summary>
    /// Constructor runs before EACH test method. Sets up the test environment.
    /// </summary>
    public CategoryServiceTests()
    {
        // Get connection string from configuration, fallback to hardcoded if not found
        string? connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback to default LocalDB connection string if not found in config
            // Ensure this path and instance name are correct for your environment.
            connectionString = @"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Robert\\ShopDBTest.mdf;Integrated Security=True;Connect Timeout=30";
        }

        // Initialize connection factory, db service, repository, and the service under test
        connectionFactory = new DbConnectionFactory(connectionString); // Initialize DbConnectionFactory
        dbService = new DbService(connectionFactory); // Initialize DbService
        categoryRepository = new CategoryRepository(dbService); // Initialize CategoryRepository (dependency for service)
        categoryService = new CategoryService(categoryRepository); // Initialize CategoryService (the service under test)

        // Setup test data in a clean database state
        using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
        try
        {
            connection.Open();

            // **CLEANUP:** Delete data from tables that Category depends on or that depend on Category.
            // Order based on your schema: referencing tables before referenced tables.
            // For Category tests, we primarily need to clear Category itself and potentially Product
            // if we are inserting test products that reference categories.
            using SqlCommand clearDataCommand = new(
                @"
                -- Delete from tables that reference Category
                DELETE FROM Product;
                -- Delete from Category itself
                DELETE FROM Category;
                ", connection);
            clearDataCommand.ExecuteNonQuery();

            // **SETUP:** Insert test data into the Category table.
            // Since Category.ID is IDENTITY, we do NOT use SET IDENTITY_INSERT ON/OFF.
            // The database will generate the ID automatically.
            using SqlCommand insertCategoryCommand = new(
                "INSERT INTO Category (Name) VALUES ('Electronics'), ('Clothing'), ('Books');", connection);
            insertCategoryCommand.ExecuteNonQuery();

            // Optional: Insert a test product that references a category if needed for specific tests
            // (e.g., testing foreign key interactions, though not strictly needed for basic Category service tests)
            // You would need to get the generated Category IDs after inserting categories if you need to reference them.
            // Example (requires getting Category IDs):
            /*
            using SqlCommand insertProductCommand = new(
                 "SET IDENTITY_INSERT Product ON; INSERT INTO Product (ID, Name, Price, Stock, CategoryID, Size, Color, Description, PhotoURL) VALUES (1, 'Test Product', 10.99, 100, (SELECT TOP 1 ID FROM Category WHERE Name = 'Electronics'), 'N/A', 'N/A', '', NULL); SET IDENTITY_INSERT Product OFF;", connection);
            insertProductCommand.ExecuteNonQuery();
            */

        }
        catch (Exception exception)
        {
            // Log the error and rethrow so the test framework knows setup failed
            Debug.WriteLine($"Failed to setup test data for CategoryServiceTests: {exception}");
            throw;
        }
    }

    // --- Test Methods for Asynchronous Operations ---

    /// <summary>
    /// Test case for retrieving all categories asynchronously.
    /// Tests the GetAllAsync method of the service.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        // Act: Call the service method under test
        var result = await categoryService.GetAllAsync();
        var categories = result.ToList();

        // Assert: Verify the results
        // We inserted 3 categories in setup
        Assert.NotNull(categories);
        Assert.Equal(3, categories.Count);
        // Add more specific assertions about the category names if needed
        Assert.Contains(categories, c => c.Name == "Electronics");
        Assert.Contains(categories, c => c.Name == "Clothing");
        Assert.Contains(categories, c => c.Name == "Books");
    }

    /// <summary>
    /// Test case for retrieving a category by a valid ID asynchronously.
    /// Tests the GetByIdAsync method of the service.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenIdExists()
    {
        // Arrange: Get an existing category ID from the database (e.g., the first one inserted)
        // Need to retrieve the ID after insertion, as it's IDENTITY
        int existingCategoryId = 0;
        using (SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection())
        {
            await connection.OpenAsync();
            using SqlCommand getIdCommand = new("SELECT TOP 1 ID FROM Category ORDER BY ID ASC", connection);
            object? result = await getIdCommand.ExecuteScalarAsync();
            if (result != null)
            {
                existingCategoryId = Convert.ToInt32(result);
            }
        }
        Assert.True(existingCategoryId > 0, "Failed to retrieve an existing category ID for test.");

        // Act: Call the service method under test with the existing ID
        var category = await categoryService.GetByIdAsync(existingCategoryId);

        // Assert: Verify the correct category is returned
        Assert.NotNull(category);
        Assert.Equal(existingCategoryId, category.ID);
        // Add assertions about the category's Name if needed
    }

    /// <summary>
    /// Test case for retrieving a category by an invalid ID asynchronously.
    /// Tests the GetByIdAsync method of the service.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
    {
        // Arrange: Use an ID that is highly unlikely to exist
        int nonExistentId = 999999;

        // Act: Call the service method under test with the non-existent ID
        var category = await categoryService.GetByIdAsync(nonExistentId);

        // Assert: Verify that null is returned
        Assert.Null(category);
    }

    /// <summary>
    /// Test case for creating a new category asynchronously.
    /// Tests the CreateAsync method of the service.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateNewCategory()
    {
        // Arrange: Create a new Category object (ID will be assigned by the DB via the repository)
        var newCategory = new Category(id: null, name: "Service Test Category");

        // Act: Call the service create method
        var createdCategory = await categoryService.CreateAsync(newCategory);

        // Assert: Verify the created category and its properties
        Assert.NotNull(createdCategory);
        Assert.True(createdCategory.ID > 0); // Ensure DB assigned an ID
        Assert.Equal("Service Test Category", createdCategory.Name);

        // Verify in database: Fetch the created category by its new ID using the repository or service
        var fetchedCategory = await categoryService.GetByIdAsync((int)createdCategory.ID); // Use service GetByIdAsync to verify
        Assert.NotNull(fetchedCategory);
        Assert.Equal(createdCategory.ID, fetchedCategory.ID);
        Assert.Equal(createdCategory.Name, fetchedCategory.Name);
    }

    /// <summary>
    /// Test case for updating an existing category asynchronously.
    /// Tests the UpdateAsync method of the service.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategory_WhenIdExists()
    {
        // Arrange: Create a category to update (ID will be assigned by DB via the service)
        var categoryToUpdate = new Category(id: null, name: "Original Service Name");
        var createdCategory = await categoryService.CreateAsync(categoryToUpdate);
        Assert.NotNull(createdCategory);
        Assert.True(createdCategory.ID > 0);

        // Modify the category object for the update
        createdCategory.Name = "Updated Service Name";

        // Act: Call the service update method
        var updatedCategory = await categoryService.UpdateAsync(createdCategory);

        // Assert: Verify the returned entity reflects the update
        Assert.NotNull(updatedCategory);
        Assert.Equal(createdCategory.ID, updatedCategory.ID);
        Assert.Equal("Updated Service Name", updatedCategory.Name);

        // Verify in database: Fetch the category from the DB and ensure it was updated
        var fetchedCategory = await categoryService.GetByIdAsync((int)updatedCategory.ID); // Use service GetByIdAsync to verify
        Assert.NotNull(fetchedCategory);
        Assert.Equal(updatedCategory.ID, fetchedCategory.ID);
        Assert.Equal("Updated Service Name", fetchedCategory.Name);
    }

    /// <summary>
    /// Test case for updating a non-existent category asynchronously.
    /// Tests the UpdateAsync method of the service.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldNotAffectDatabase_WhenIdDoesNotExist()
    {
        // Arrange: Create a category object with a non-existent ID
        int nonExistentId = 999999;
        var nonExistentCategory = new Category(id: nonExistentId, name: "Should Not Exist Service");

        // Act: Call the service update method
        var updatedCategory = await categoryService.UpdateAsync(nonExistentCategory);
        // Assert: The returned entity should match the input (as the service/repo returns the input)

        Assert.NotNull(updatedCategory);
        Assert.Equal(nonExistentCategory.ID, updatedCategory.ID);
        Assert.Equal(nonExistentCategory.Name, updatedCategory.Name);

        // Verify in database: Ensure no category with this ID was created or affected
        var fetchedCategory = await categoryService.GetByIdAsync((int)nonExistentCategory.ID); // Use service GetByIdAsync to verify
        Assert.Null(fetchedCategory);
    }


    /// <summary>
    /// Test case for deleting a category by its ID asynchronously.
    /// Tests the DeleteAsync method of the service (hard delete).
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldDeleteCategory_WhenIdExists()
    {
        // Arrange: Create a category to delete (ID will be assigned by DB via the service)
        var categoryToDelete = new Category(id: null, name: "To Be Deleted Service");
        var createdCategory = await categoryService.CreateAsync(categoryToDelete);
        Assert.NotNull(createdCategory);
        Assert.True(createdCategory.ID > 0);

        // Act: Call the service delete method
        var deleteResult = await categoryService.DeleteAsync((int)createdCategory.ID); // Use int id

        // Assert: Verify the method returned true
        Assert.True(deleteResult);

        // Verify in database: Ensure the category is no longer found
        var fetchedCategory = await categoryService.GetByIdAsync((int)createdCategory.ID); // Use service GetByIdAsync to verify
        Assert.Null(fetchedCategory);
    }

    /// <summary>
    /// Test case for deleting a non-existent category by ID asynchronously.
    /// Tests the DeleteAsync method of the service.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenIdDoesNotExist()
    {
        // Arrange: Use an ID that is highly unlikely to exist
        int nonExistentId = 999999;

        // Act: Call the service delete method
        var deleteResult = await categoryService.DeleteAsync(nonExistentId); // Use int id

        // Assert: Verify the method returned false (as no row was affected)
        Assert.False(deleteResult);
    }


    // --- Dispose Method ---
    /// <summary>
    /// Dispose method runs automatically after EACH test method. Cleans up the test environment.
    /// </summary>
    public void Dispose() // Implement Dispose from IDisposable
    {
        // Cleanup is primarily done at the start of the constructor to ensure a clean state BEFORE each test.
        // However, keeping this cleanup here as a failsafe doesn't hurt and ensures the database
        // is cleaned up even if a test fails after the constructor completes.
        using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
        try
        {
            connection.Open();
            // Delete data from tables that Category depends on or that depend on Category.
            // Order based on your schema: referencing tables before referenced tables.
            using SqlCommand clearDataCommand = new(
                @"
                DELETE FROM Product; -- Delete products that might reference categories
                DELETE FROM Category; -- Delete categories themselves
                ", connection);
            clearDataCommand.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            // Log cleanup errors but avoid rethrowing in Dispose if possible,
            // to prevent masking the original test failure.
            Debug.WriteLine($"Failed to delete test data during Dispose: {exception}");
            // Depending on your test framework and desired behavior, you might just log here.
            // For simplicity and to ensure cleanup issues are visible, we'll rethrow.
            throw;
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }
}
