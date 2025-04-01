using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;

namespace WorkoutApp.Repository
{
    public class ProductRepository
    {
        public ProductRepository() { }
        public List<Product> GetAll()
        {
            var products = new List<Product>
            {
                new Product { Name = "Tricou Negru XL", Price = "29.99", Quantity = "2", Image = "./photos/tricou.jpg" },
                new Product { Name = "Gantera 10 KG", Price = "49.99", Quantity = "1", Image = "./photos/gantera.jpg" },
                new Product { Name = "Proteina Gym Beam", Price = "19.99", Quantity = "3", Image = "./photos/protein.jpg" },
            };

            return products;
        }

        public Product GetProductById(int id)
        {
            if (id == 0)
            {
                return new Product { Name = "Tricou Negru XL", Price = "29.99", Quantity = "2", Image = "./photos/tricou.jpg" };
            }
            if (id == 1)
            {
                return new Product { Name = "Gantera 10 KG", Price = "49.99", Quantity = "1", Image = "./photos/gantera.jpg" };
            }
            else
            {
                return new Product { Name = "Proteina Gym Beam", Price = "19.99", Quantity = "3", Image = "./photos/protein.jpg" };
            }
        }

    }
}