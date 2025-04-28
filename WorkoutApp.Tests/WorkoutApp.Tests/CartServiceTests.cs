using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Data.Database;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using static System.Collections.Specialized.BitVector32;

namespace WorkoutApp.Tests
{
    public class CartServiceTests
    {
        private readonly CartService cartService;
        private readonly CartItemRepository cartItemRepository;
        private readonly ProductRepository productRepository;
        private readonly DbService dbService;
        private readonly DbConnectionFactory connectionFactory;

        public CartServiceTests()
        {
            string? connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;
            TestDbConnectionFactory testDbConnectionFactory = new TestDbConnectionFactory(connectionString);
            this.dbService = new DbService(testDbConnectionFactory);
            this.cartItemRepository = new CartItemRepository(this.dbService);
            this.productRepository = new ProductRepository();
            this.cartService = new CartService(this.cartItemRepository, this.productRepository);
        }

        private async Task InitializeAsync()
        {
            // Clear existing data (important to avoid duplicates)
            await dbService.ExecuteQueryAsync("DELETE FROM OrderDetail", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DELETE FROM [Order]", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DELETE FROM CartItem", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DELETE FROM Wishlist", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DELETE FROM Cart", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DELETE FROM Product", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DELETE FROM Category", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DELETE FROM Customer", new List<SqlParameter>());

            // Reseed identity (optional but nice for predictable IDs)
            await dbService.ExecuteQueryAsync("DBCC CHECKIDENT ('OrderDetail', RESEED, 0)", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DBCC CHECKIDENT ('[Order]', RESEED, 0)", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DBCC CHECKIDENT ('CartItem', RESEED, 0)", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DBCC CHECKIDENT ('Wishlist', RESEED, 0)", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DBCC CHECKIDENT ('Cart', RESEED, 0)", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DBCC CHECKIDENT ('Product', RESEED, 0)", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DBCC CHECKIDENT ('Category', RESEED, 0)", new List<SqlParameter>());
            await dbService.ExecuteQueryAsync("DBCC CHECKIDENT ('Customer', RESEED, 0)", new List<SqlParameter>());

            // Insert fresh data
            // Insert Customer
            await dbService.ExecuteQueryAsync(
                "INSERT INTO Customer (IsActive) VALUES (1)",
                new List<SqlParameter>());

            // Insert Categories
            await dbService.ExecuteQueryAsync(
                "INSERT INTO Category (Name, IsActive) VALUES " +
                "('Fitness Equipment', 1), " +
                "('Supplements', 1), " +
                "('Clothing', 1)",
                new List<SqlParameter>());

            // Insert Products
            await dbService.ExecuteQueryAsync(
                "INSERT INTO Product (Name, Price, Stock, CategoryID, Atributes, Size, Description, FileUrl, IsActive) VALUES " +
                "('Yoga Mat', 29.99, 100, 1, 'Blue', 'Standard', 'High quality yoga mat', 'url_to_image.jpg', 1)," +
                "('Whey Protein', 49.99, 50, 2, NULL, '2kg', 'Premium whey protein', 'url_to_protein.jpg', 1)," +
                "('Running Shoes', 79.99, 30, 3, 'Black', '42', 'Comfortable running shoes', 'url_to_shoes.jpg', 1)",
                new List<SqlParameter>());

            // Insert Cart
            await dbService.ExecuteQueryAsync(
                "INSERT INTO Cart (CustomerID, CreatedAt, IsActive) VALUES (1, GETDATE(), 1)",
                new List<SqlParameter>());

            // Insert CartItems
            await dbService.ExecuteQueryAsync(
                "INSERT INTO CartItem (CartID, ProductID, Quantity, IsActive) VALUES " +
                "(1, 1, 2, 1), " +
                "(1, 2, 1, 1)",
                new List<SqlParameter>());

            // Insert Order
            await dbService.ExecuteQueryAsync(
                "INSERT INTO [Order] (CustomerID, OrderDate, TotalAmount, IsActive) VALUES (1, GETDATE(), 109.97, 1)",
                new List<SqlParameter>());

            // Insert OrderDetails
            await dbService.ExecuteQueryAsync(
                "INSERT INTO OrderDetail (OrderID, ProductID, Quantity, Price, IsActive) VALUES " +
                "(1, 1, 2, 29.99, 1), " +
                "(1, 2, 1, 49.99, 1)",
                new List<SqlParameter>());

            // Insert Wishlist
            await dbService.ExecuteQueryAsync(
                "INSERT INTO Wishlist (ProductID, CustomerID, IsActive) VALUES (3, 1, 1)",
                new List<SqlParameter>());
        }

        [Fact]
        public async Task GetCartItemAsync_ShouldReturnAllCartItems()
        {
            // Arrange
            await this.InitializeAsync(); 

            // Act
            var cartItems = await this.cartService.GetCartItemAsync();

            // Assert
            Assert.NotNull(cartItems);
            Assert.Equal(2, cartItems.Count); // You inserted 2 items (Yoga Mat x2, Whey Protein x1)
        }

        [Fact]
        public async Task GetCartItemByIdAsync_ShouldReturnCorrectItem()
        {
            // Arrange
            await this.InitializeAsync();

            // Act
            var cartItem = await this.cartService.GetCartItemByIdAsync(1);

            // Assert
            Assert.NotNull(cartItem);
            Assert.Equal(2, cartItem.Quantity); // Yoga Mat x2
        }

        [Fact]
        public async Task IncreaseQuantityAsync_ShouldIncreaseQuantityByOne()
        {
            // Arrange
            await this.InitializeAsync();
            var cartItem = await this.cartService.GetCartItemByIdAsync(1);
            int initialQuantity = (int)cartItem.Quantity;

            // Act
            await this.cartService.IncreaseQuantityAsync(cartItem);
            var updatedItem = await this.cartService.GetCartItemByIdAsync(1);

            // Assert
            Assert.Equal(initialQuantity + 1, updatedItem.Quantity);
        }

        [Fact]
        public async Task DecreaseQuantityAsync_ShouldDecreaseQuantityByOne()
        {
            // Arrange
            await this.InitializeAsync();
            var cartItem = await this.cartService.GetCartItemByIdAsync(1);
            int initialQuantity = (int)cartItem.Quantity;

            // Act
            await this.cartService.DecreaseQuantityAsync(cartItem);
            var updatedItem = await this.cartService.GetCartItemByIdAsync(1);

            // Assert
            Assert.Equal(initialQuantity - 1, updatedItem.Quantity);
        }


        [Fact]
        public async Task RemoveCartItemAsync_ShouldDeleteItem()
        {
            // Arrange
            await this.InitializeAsync();
            var cartItem = await this.cartService.GetCartItemByIdAsync(2);

            // Act
            await this.cartService.RemoveCartItemAsync(cartItem);

            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await this.cartService.GetCartItemByIdAsync((int)cartItem.Id));
        }

        [Fact]
        public async Task AddToCartAsync_ShouldAddNewItem()
        {
            // Arrange
            await this.InitializeAsync();
            var initialCartItems = await this.cartService.GetCartItemAsync();

            // Act
            await this.cartService.AddToCartAsync(3, 2); // Add Running Shoes x2
            var updatedCartItems = await this.cartService.GetCartItemAsync();

            // Assert
            Assert.Equal(initialCartItems.Count + 1, updatedCartItems.Count);

            var newItem = updatedCartItems.Last();
            Assert.Equal(3, newItem.ProductId); // Running Shoes ProductID
            Assert.Equal(2, newItem.Quantity);
        }
    }
}
