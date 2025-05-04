using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using WorkoutApp.Repository;
using Moq;
using WorkoutApp.Utils.Filters;
using WorkoutApp.Data.Database;
using WorkoutApp.Models;

namespace WorkoutApp.Tests.Repository
{
    [Collection("DatabaseTests")]
    public class IRepositoryTests
    {
        [Fact]
        public async Task GetAllFilteredAsync_ShouldReturnEmptyCollection()
        {
            // Arrange
            IRepository<Category> categoryRepository = new CategoryRepository(new DbService(new DbConnectionFactory("")));

            // Act
            var result = await categoryRepository.GetAllFilteredAsync(new ProductFilter(null, null, null, null, null, null));

            // Assert
            Assert.Empty(result);
        }
    }
}
