using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutApp.Models
{
    public class ClothesProduct : IProduct
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public int CategoryID { get; set; } 
        public string Attributes { get; set; } 
        public string Size { get; set; }
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public bool IsActive { get; set; }

        public ClothesProduct(int id, string name, double price, int stock, int categoryId, string color, string size, string description, string fileUrl, bool isActive)
        {
            ID = id;
            Name = name;
            Price = price;
            Stock = stock;
            CategoryID = categoryId;
            Attributes = color;
            Size = size;
            Description = description;
            FileUrl = fileUrl;
            IsActive = isActive;
        }

    }
}
