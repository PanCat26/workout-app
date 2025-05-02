using Moq;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;

namespace WorkoutApp.Tests.Service
{
    [Collection("DatabaseTests")]
    public class ProductServiceTests
    {
        private readonly Mock<IRepository<Product>> productRepositoryMock;
        private readonly ProductService productService;

        public ProductServiceTests()
        {
            productRepositoryMock = new Mock<IRepository<Product>>();
            productService = new ProductService(productRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedProduct()
        {
            var product = new Product(1, "Test Product", 10.99m, 5, new Category(1, "Accessories"), "M", "Black", "Great product", "http://example.com/image.jpg");
            productRepositoryMock.Setup(repo => repo.CreateAsync(product)).ReturnsAsync(product);

            var result = await productService.CreateAsync(product);

            Assert.Equal(product, result);
            productRepositoryMock.Verify(repo => repo.CreateAsync(product), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeletionSucceeds()
        {
            int productId = 1;
            productRepositoryMock.Setup(repo => repo.DeleteAsync(productId)).ReturnsAsync(true);

            var result = await productService.DeleteAsync(productId);

            Assert.True(result);
            productRepositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            var products = new List<Product>
            {
                new Product(1, "A", 10m, 5, new Category (1, "Category A"), "L", "Red", "Desc A", "http://example.com/a.jpg"),
                new Product(2, "B", 20m, 10, new Category (2, "Category B"), "XL", "Blue", "Desc B", "http://example.com/b.jpg")
            };
            productRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            var result = await productService.GetAllAsync();

            Assert.Equal(2, result.Count());
            productRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectProduct()
        {
            var product = new Product(1, "Product A", 15.99m, 3, new Category(3, "Category C"), "S", "Green", "Nice product", "http://example.com/c.jpg");
            productRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await productService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Product A", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedProduct()
        {
            var product = new Product(1, "Updated Product", 30m, 7, new Category(1, "Accessories"), "M", "Black", "Updated description", "http://example.com/image.jpg");
            productRepositoryMock.Setup(repo => repo.UpdateAsync(product)).ReturnsAsync(product);

            var result = await productService.UpdateAsync(product);

            Assert.Equal("Updated Product", result.Name);
            Assert.Equal(30m, result.Price);
        }
    }
}
