using Microsoft.Data.SqlClient;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using System.Configuration;
using WorkoutApp.Data.Database;
using WorkoutApp.Utils.Filters;

namespace WorkoutApp.Tests.Repository
{
    [Collection("DatabaseTests")]
    public class ProductRepositoryTests : IDisposable
    {
        private readonly DbService dbService;
        private readonly ProductRepository repository;

        public ProductRepositoryTests()
        {
            string? testConnectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(testConnectionString))
            {
                throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
            }

            DbConnectionFactory dbConnectionFactory = new(testConnectionString);
            dbService = new(dbConnectionFactory);

            try
            {
                // Reset ids in test database
                string resetIdsQuery = "DBCC CHECKIDENT ('Product', RESEED, 0); DBCC CHECKIDENT ('Category', RESEED, 0);";
                dbService.ExecuteQueryAsync(resetIdsQuery, []).Wait();

                string insertCategoryQuery = "INSERT INTO Category (Name) VALUES (@Name);";
                List<SqlParameter> categoryParameters =
                [
                    new ("@Name", "Test Category"),
                ];
                dbService.ExecuteQueryAsync(insertCategoryQuery, categoryParameters).Wait();

                string insertProductQuery = @"
                   INSERT INTO Product (Name, Price, Stock, CategoryId, Size, Color, Description, PhotoURL)
                   VALUES 
                   (@Name1, @Price1, @Stock1, @CategoryID1, @Size1, @Color1, @Description1, @PhotoURL1),
                   (@Name2, @Price2, @Stock2, @CategoryID2, @Size2, @Color2, @Description2, @PhotoURL2)";
                List<SqlParameter> parameters =
                [
                    new ("@Name1", "Test Product 1"),
                    new ("@Price1", 10.99m),
                    new ("@Stock1", 100),
                    new ("@CategoryID1", 1),
                    new ("@Size1", "M"),
                    new ("@Color1", "Red"),
                    new ("@Description1", "Description for Test Product 1"),
                    new ("@PhotoURL1", "http://example.com/product1.jpg"),

                    new ("@Name2", "Test Product 2"),
                    new ("@Price2", 15.99m),
                    new ("@Stock2", 50),
                    new ("@CategoryID2", 1),
                    new ("@Size2", "L"),
                    new ("@Color2", "Blue"),
                    new ("@Description2", "Description for Test Product 2"),
                    new ("@PhotoURL2", "http://example.com/product2.jpg"),
                ];
                dbService.ExecuteQueryAsync(insertProductQuery, parameters).Wait();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database setup failed: " + ex.Message, ex);
            }

            repository = new ProductRepository(dbService);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts()
        {
            List<Product> result = (List<Product>)await repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);

            // Validate the first product
            Product product1 = result.FirstOrDefault(p => p.Name == "Test Product 1")!;
            Assert.Equal("Test Product 1", product1.Name);
            Assert.Equal(10.99m, product1.Price);
            Assert.Equal(100, product1.Stock);
            Assert.Equal("M", product1.Size);
            Assert.Equal("Red", product1.Color);
            Assert.Equal("Description for Test Product 1", product1.Description);
            Assert.Equal("http://example.com/product1.jpg", product1.PhotoURL);

            // Validate the second product
            Product product2 = result.FirstOrDefault(p => p.Name == "Test Product 2")!;
            Assert.Equal("Test Product 2", product2.Name);
            Assert.Equal(15.99m, product2.Price);
            Assert.Equal(50, product2.Stock);
            Assert.Equal("L", product2.Size);
            Assert.Equal("Blue", product2.Color);
            Assert.Equal("Description for Test Product 2", product2.Description);
            Assert.Equal("http://example.com/product2.jpg", product2.PhotoURL);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            Product? result = await repository.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("Test Product 1", result.Name);
            Assert.Equal(10.99m, result.Price);
            Assert.Equal(100, result.Stock);
            Assert.Equal("M", result.Size);
            Assert.Equal("Red", result.Color);
            Assert.Equal("Description for Test Product 1", result.Description);
            Assert.Equal("http://example.com/product1.jpg", result.PhotoURL);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            Product? result = await repository.GetByIdAsync(999999); // Non-existing product ID

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateProduct_WhenValidProductIsProvided()
        {
            Product newProduct = new(
                id: 3,
                name: "New Product",
                price: 25.00m,
                stock: 10,
                category: new Category(1, "Test Category"),
                color: "Black",
                size: "L",
                description: "New Description",
                photoURL: "http://example.com/new.jpg");
            Product createdProduct = await repository.CreateAsync(newProduct);
            Assert.NotNull(createdProduct);
            Assert.Equal("New Product", createdProduct.Name);
            Assert.Equal(25.00m, createdProduct.Price);
            Assert.Equal(10, createdProduct.Stock);
            Assert.Equal("Test Category", createdProduct.Category.Name);
            Assert.Equal("Black", createdProduct.Color);
            Assert.Equal("L", createdProduct.Size);
            Assert.Equal("New Description", createdProduct.Description);
            Assert.Equal("http://example.com/new.jpg", createdProduct.PhotoURL);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
        {
            Product productToUpdate = new(
                id: 1,
                name: "Updated Product",
                price: 20.00m,
                stock: 15,
                category: new Category(1, "Test Category"),
                color: "Green",
                size: "XL",
                description: "Updated Description",
                photoURL: "http://example.com/updated.jpg");

            Product updatedProduct = await repository.UpdateAsync(productToUpdate);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product", updatedProduct.Name);
            Assert.Equal(20.00m, updatedProduct.Price);
            Assert.Equal(15, updatedProduct.Stock);
            Assert.Equal("Test Category", updatedProduct.Category.Name);
            Assert.Equal("Green", updatedProduct.Color);
            Assert.Equal("XL", updatedProduct.Size);
            Assert.Equal("Updated Description", updatedProduct.Description);
            Assert.Equal("http://example.com/updated.jpg", updatedProduct.PhotoURL);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowArgumentException_WhenProductIdIsNotGiven()
        {
            Product productToUpdate = new(
                id: null, // Null ID
                name: "Invalid Product",
                price: 0.00m,
                stock: 0,
                category: new Category(1, "Test Category"),
                color: "Black",
                size: "M",
                description: "Invalid Description",
                photoURL: "http://example.com/invalid.jpg");
            await Assert.ThrowsAsync<ArgumentException>(async () => await repository.UpdateAsync(productToUpdate));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenProductDoesNotExist()
        {
            Product productToUpdate = new(
                id: 999999, // Non-existing product ID
                name: "Non-existing Product",
                price: 0.00m,
                stock: 0,
                category: new Category(1, "Test Category"),
                color: "Black",
                size: "M",
                description: "Non-existing Description",
                photoURL: "http://example.com/non-existing.jpg");

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.UpdateAsync(productToUpdate));
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenProductExists()
        {
            //get a list of all product ids for debugging

            bool result = await repository.DeleteAsync(1); // Existing product ID
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            bool result = await repository.DeleteAsync(999999); // Non-existing product ID

            Assert.False(result);
        }

        [Fact]
        public async Task GetAllFilteredAsync_ShouldReturnFilteredResults_WhenAllFilterValuesSet()
        {
            // Arrange
            var filter = new ProductFilter(
                categoryId: 1,
                excludeProductId: 999, // doesn't exist so won't affect result
                count: 1, // limit to 1
                color: "Red",
                size: "M",
                searchTerm: "Test Product 1"
            );

            // Act
            var result = await repository.GetAllFilteredAsync(filter);
            var list = result.ToList();

            // Assert
            Assert.Single(list);
            Assert.Equal("Test Product 1", list[0].Name);
            Assert.Equal("Red", list[0].Color);
            Assert.Equal("M", list[0].Size);
            Assert.Contains("Test Product 1", list[0].Name);
        }

        [Fact]
        public async Task GetAllFilteredAsync_ShouldReturnAll_WhenFilterIsEmpty()
        {
            // Arrange
            var filter = new ProductFilter(null, null, null, null, null, null);

            // Act
            var result = await repository.GetAllFilteredAsync(filter);
            var list = result.ToList();

            // Assert
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);
        }

        public void Dispose()
        {
            string cleanupQuery = "DELETE FROM Product; DELETE FROM Category;";
            try
            {
                dbService.ExecuteQueryAsync(cleanupQuery, []).Wait();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database cleanup failed: " + ex.Message, ex);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
