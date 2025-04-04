using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string FileUrl { get; set; }

        public string Type { get; set; }

        public string Quantity { get; set; }

        public List<string>? Colors { get; set; }
        public List<string>? Sizes { get; set; }
        public List<string>? Weights { get; set; }
    }
}
