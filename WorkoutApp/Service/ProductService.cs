/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.Services.Store;
using WorkoutApp.Repository;
using WorkoutApp.Models;

namespace WorkoutApp.Service
{
    public class ProductService
    {
        private ProductRepository repository;


        public ProductService(ProductRepository repository)
        {
            this.repository = repository;
        }

        public void ValidateProduct(int productId, string name, double price, int stock, int categoryId, string description, string fileUrl, string color = null, string size = null)
        {
            if (name == null || name.Length == 0)
            {
                throw new Exception("Name cannot be null");

            }
            if (price <= 0)
            {
                throw new Exception("Price must be greater than zero");
            }
            if (string.IsNullOrEmpty(fileUrl))
            {
                throw new Exception("URL cannot be null or empty");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new Exception("Description cannot be null or empty");
            }

            if (stock < 0)
            {
                throw new Exception("Quantity cannot be negative");
            }

            if (categoryId != 1 && categoryId != 2 && categoryId != 3)
            {
                throw new Exception("Invalid category ID");
            }
            if (categoryId == 1)
            {

                if (string.IsNullOrEmpty(size))
                {
                    throw new Exception("Size cannot be null or empty");
                }

                if (string.IsNullOrEmpty(color))
                {
                    throw new Exception("Color cannot be null or empty");
                }


            }
            else if (categoryId == 2)
            {
                if (string.IsNullOrEmpty(size))
                {
                    throw new Exception("Size cannot be null or empty");
                }

            }


        }

      

        public void UpdateProduct(int productId, string name, double price, int stock, int categoryId, string description, string fileUrl, string color, string size)
        {
            ValidateProduct(productId, name, price, stock, categoryId, description, fileUrl, color, size);
            if (repository.GetById(productId) == null)
            {
                throw new Exception("Product does not exist");
            }
            repository.UpdateProduct(productId, name, price, stock, categoryId, description, fileUrl, color, size);
        }

        public void DeleteProduct(int productID)
        {
            if (repository.GetById(productID) == null)
            {
                throw new Exception("Product does not exist");
            }
            repository.DeleteById(productID);
        }

        public List<IProduct> GetAll()
        {
            return repository.GetAll();
        }

        public IProduct GetById(int productID)
        {
            return repository.GetById(productID);
        }


        public List<IProduct> GetRecommendedProducts(int productID)
        {
            IProduct product = repository.GetById(productID);
            List<IProduct> allProducts = repository.GetAll();

            List<IProduct> recommendedProducts =allProducts
                .Where(p => p.ID != product.ID 
                    && p.CategoryID == product.CategoryID 
                    && !p.Name.Equals(product.Name, StringComparison.Ordinal) 
                    && !p.Name.Contains(product.Name, StringComparison.OrdinalIgnoreCase) 
        )
        .Take(3) 
        .ToList();

            return recommendedProducts;
        }





    }
}
*/