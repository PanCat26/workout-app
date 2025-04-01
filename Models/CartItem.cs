using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Repository;

namespace WorkoutApp.Models
{
    public class CartItem
    {
        public long Id {  get; set; }
        public long CartId { get; set; }
        public long ProductId { get; set; }
        public long Quantity { get; set; }

        public Product GetProduct(ProductRepository repo)
        {
            return repo.GetProductById((int)ProductId);
        }
    }
}
