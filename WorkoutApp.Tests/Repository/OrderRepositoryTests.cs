// <copyright file="OrderRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Tests.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Moq;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;
    using Xunit;

    /// <summary>
    /// Contains tests for the OrderRepository class.
    /// </summary>
    public class OrderRepositoryTests
    {
        private readonly Mock<DbService> mockDbService;
        private readonly OrderRepository orderRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepositoryTests"/> class.
        /// </summary>
        public OrderRepositoryTests()
        {
            this.mockDbService = new Mock<DbService>(MockBehavior.Strict, new Mock<DbConnectionFactory>().Object);
            this.orderRepository = new OrderRepository(this.mockDbService.Object);
        }

        /// <summary>
        /// Tests that GetAllAsync returns all active orders.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsAllActiveOrders()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("CustomerID", typeof(int));
            dataTable.Columns.Add("OrderDate", typeof(DateTime));
            dataTable.Columns.Add("TotalAmount", typeof(double));
            dataTable.Columns.Add("IsActive", typeof(bool));

            dataTable.Rows.Add(1, 1, DateTime.Now, 100.0, true);
            dataTable.Rows.Add(2, 1, DateTime.Now, 200.0, true);

            this.mockDbService.Setup(x => x.ExecuteSelectAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()))
                .ReturnsAsync(dataTable);

            // Act
            var result = await this.orderRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            this.mockDbService.Verify(x => x.ExecuteSelectAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()), Times.Once);
        }

        /// <summary>
        /// Tests that GetByIdAsync returns the correct order.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectOrder()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("CustomerID", typeof(int));
            dataTable.Columns.Add("OrderDate", typeof(DateTime));
            dataTable.Columns.Add("TotalAmount", typeof(double));
            dataTable.Columns.Add("IsActive", typeof(bool));

            var expectedDate = DateTime.Now;
            dataTable.Rows.Add(1, 1, expectedDate, 100.0, true);

            this.mockDbService.Setup(x => x.ExecuteSelectAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()))
                .ReturnsAsync(dataTable);

            // Act
            var result = await this.orderRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ID);
            Assert.Equal(1, result.CustomerID);
            Assert.Equal(100.0, result.TotalAmount);
            Assert.True(result.IsActive);
            this.mockDbService.Verify(x => x.ExecuteSelectAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()), Times.Once);
        }

        /// <summary>
        /// Tests that GetByIdAsync returns null when order not found.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenOrderNotFound()
        {
            // Arrange
            var dataTable = new DataTable();
            this.mockDbService.Setup(x => x.ExecuteSelectAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()))
                .ReturnsAsync(dataTable);

            // Act
            var result = await this.orderRepository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            this.mockDbService.Verify(x => x.ExecuteSelectAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()), Times.Once);
        }

        /// <summary>
        /// Tests that CreateAsync creates a new order.
        /// </summary>
        [Fact]
        public async Task CreateAsync_CreatesNewOrder()
        {
            // Arrange
            var order = new Order(0, 1, DateTime.Now, 100.0, true);
            this.mockDbService.Setup(x => x.ExecuteQueryAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()))
                .ReturnsAsync(1);

            // Act
            var result = await this.orderRepository.CreateAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.CustomerID, result.CustomerID);
            Assert.Equal(order.TotalAmount, result.TotalAmount);
            this.mockDbService.Verify(x => x.ExecuteQueryAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()), Times.Once);
        }

        /// <summary>
        /// Tests that UpdateAsync updates an existing order.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_UpdatesExistingOrder()
        {
            // Arrange
            var order = new Order(1, 1, DateTime.Now, 200.0, true);
            this.mockDbService.Setup(x => x.ExecuteQueryAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()))
                .ReturnsAsync(1);

            // Act
            var result = await this.orderRepository.UpdateAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.ID, result.ID);
            Assert.Equal(order.TotalAmount, result.TotalAmount);
            this.mockDbService.Verify(x => x.ExecuteQueryAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()), Times.Once);
        }

        /// <summary>
        /// Tests that DeleteAsync soft deletes an order.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_SoftDeletesOrder()
        {
            // Arrange
            this.mockDbService.Setup(x => x.ExecuteQueryAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()))
                .ReturnsAsync(1);

            // Act
            var result = await this.orderRepository.DeleteAsync(1);

            // Assert
            Assert.True(result);
            this.mockDbService.Verify(x => x.ExecuteQueryAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()), Times.Once);
        }

        /// <summary>
        /// Tests that DeleteAsync returns false when order not found.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenOrderNotFound()
        {
            // Arrange
            this.mockDbService.Setup(x => x.ExecuteQueryAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()))
                .ReturnsAsync(0);

            // Act
            var result = await this.orderRepository.DeleteAsync(999);

            // Assert
            Assert.False(result);
            this.mockDbService.Verify(x => x.ExecuteQueryAsync(
                It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>()), Times.Once);
        }
    }
} 