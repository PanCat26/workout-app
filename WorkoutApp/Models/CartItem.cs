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
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public IProduct GetProduct(ProductRepository repo)
        {
            repo.LoadData();
            return repo.GetById((int) ProductId);
        }
    }
}