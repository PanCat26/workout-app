/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutApp.Models;
using WorkoutApp.Repository;

namespace WorkoutApp.Service
{
    public class ProductService
    {
        private readonly ProductRepository repository;

        public ProductService(ProductRepository repository)
        {
            this.repository = repository;
        }

        public void ValidateProduct(int productId, string name, double price, int stock, int categoryId, string description, string fileUrl, string color = null, string size = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name cannot be null");
            if (price <= 0)
                throw new Exception("Price must be greater than zero");
            if (string.IsNullOrEmpty(fileUrl))
                throw new Exception("URL cannot be null or empty");
            if (string.IsNullOrEmpty(description))
                throw new Exception("Description cannot be null or empty");
            if (stock < 0)
                throw new Exception("Quantity cannot be negative");
            if (categoryId != 1 && categoryId != 2 && categoryId != 3)
                throw new Exception("Invalid category ID");

            if (categoryId == 1)
            {
                if (string.IsNullOrEmpty(size))
                    throw new Exception("Size cannot be null or empty");
                if (string.IsNullOrEmpty(color))
                    throw new Exception("Color cannot be null or empty");
            }
            else if (categoryId == 2)
            {
                if (string.IsNullOrEmpty(size))
                    throw new Exception("Size cannot be null or empty");
            }
        }

        public async Task UpdateProductAsync(int productId, string name, double price, int stock, int categoryId, string description, string fileUrl, string color, string size)
        {
            ValidateProduct(productId, name, price, stock, categoryId, description, fileUrl, color, size);

            var product = await repository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product does not exist");

            product.Name = name;
            product.Price = price;
            product.Stock = stock;
            product.CategoryID = categoryId;
            product.Description = description;
            product.FileUrl = fileUrl;

            if (product is ClothesProduct clothes)
            {
                clothes.Attributes = color;
                clothes.Size = size;
            }
            else if (product is FoodProduct food)
            {
                food.Size = size;
            }

            await repository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await repository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product does not exist");

            await repository.DeleteAsync(productId);
        }

        public async Task<List<IProduct>> GetAllAsync()
        {
            var products = await repository.GetAllAsync();
            return products.ToList();
        }

        public async Task<IProduct> GetByIdAsync(int productId)
        {
            return await repository.GetByIdAsync(productId);
        }

        public async Task<List<IProduct>> GetRecommendedProductsAsync(int productId)
        {
            var product = await repository.GetByIdAsync(productId);
            if (product == null)
                return new List<IProduct>();

            var allProducts = (await repository.GetAllAsync()).ToList();

            var recommended = allProducts
                .Where(p => p.ID != product.ID &&
                            p.CategoryID == product.CategoryID &&
                            !p.Name.Equals(product.Name, StringComparison.Ordinal) &&
                            !p.Name.Contains(product.Name, StringComparison.OrdinalIgnoreCase))
                .Take(3)
                .ToList();

            return recommended;
        }
    }
}
*/