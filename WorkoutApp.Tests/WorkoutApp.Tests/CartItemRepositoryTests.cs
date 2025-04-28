using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WorkoutApp.Data.Database;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using Xunit;

public class CartItemRepositoryTests
{
    private readonly DbService dbService;
    private readonly CartItemRepository cartItemRepository;
    private readonly DbConnectionFactory connectionFactory;
    private int productId;
    private int cartId;

    public CartItemRepositoryTests()
    {
        string? connectionString = ConfigurationManager.ConnectionStrings["TestConnection"]?.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("TestConnection string is missing or empty in config file.");
        }

        connectionFactory = new TestDbConnectionFactory(connectionString);
        dbService = new DbService(connectionFactory);

        using SqlConnection connection = (SqlConnection)connectionFactory.CreateConnection();
        try
        {
            connection.Open();
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Failed to open SQL connection: {exception}");
            throw;
        }
        this.cartItemRepository = new CartItemRepository(dbService);
        productId = 1;
    }


    public async Task InitializeAsync()
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
    public async Task GetAllAsync_ShouldReturnCartItems()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var items = await cartItemRepository.GetAllAsync();

        // Assert
        Assert.NotNull(items);
        Assert.NotEmpty(items); 
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectCartItem()
    {
        // Arrange
        await InitializeAsync();
        var allItems = await cartItemRepository.GetAllAsync();
        var firstItem = await cartItemRepository.GetByIdAsync(allItems.First().Id);

        // Assert
        Assert.NotNull(firstItem);
        Assert.Equal(allItems.First().Id, firstItem.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewCartItem()
    {
        // Arrange
        await InitializeAsync();
        int productId = 3; 
        int quantity = 5;

        // Act
        await cartItemRepository.CreateAsync(productId, quantity);
        var allItems = await cartItemRepository.GetAllAsync();

        // Assert
        Assert.Contains(allItems, item => item.ProductId == productId && item.Quantity == quantity);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCartItemQuantity()
    {
        // Arrange
        await InitializeAsync();
        var allItems = await cartItemRepository.GetAllAsync();
        var itemToUpdate = allItems.First();
        itemToUpdate.Quantity = itemToUpdate.Quantity + 1;

        // Act
        var updatedItem = await cartItemRepository.UpdateAsync(itemToUpdate);
        var refreshedItem = await cartItemRepository.GetByIdAsync(itemToUpdate.Id);

        // Assert
        Assert.Equal(updatedItem.Quantity, refreshedItem.Quantity);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteCartItem()
    {
        // Arrange
        await InitializeAsync();
        var allItems = await cartItemRepository.GetAllAsync();
        var itemToDelete = allItems.First();

        // Act
        bool deleteResult = await cartItemRepository.DeleteAsync(itemToDelete.Id);
        var allItemsAfterDelete = await cartItemRepository.GetAllAsync();

        // Assert
        Assert.True(deleteResult);
        Assert.DoesNotContain(allItemsAfterDelete, item => item.Id == itemToDelete.Id);
    }

    [Fact]
    public async Task ResetCart_ShouldCreateNewCartAndDeactivateOld()
    {
        // Arrange
        await InitializeAsync();
        var activeCartIdBefore = await GetActiveCartId();

        // Act
        bool result = await cartItemRepository.ResetCart();
        var activeCartIdAfter = await GetActiveCartId();

        // Assert
        Assert.True(result);
        Assert.True(activeCartIdAfter > activeCartIdBefore);
    }

    private async Task<int> GetActiveCartId()
    {
        var result = await dbService.ExecuteSelectAsync(
            "SELECT ISNULL(MAX(ID), 0) FROM Cart",
            new List<SqlParameter>());

        return Convert.ToInt32(result.Rows[0][0]);
    }
}

