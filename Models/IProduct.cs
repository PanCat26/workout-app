using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutApp.Models
{
    public interface IProduct
    {
        int ID { get; set; }
        string Name { get; set; }
        double Price { get; set; }
        int Stock { get; set; }
        int CategoryID { get; set; }
        string Description { get; set; }
        string FileUrl { get; set; }
         bool IsActive { get; set; }
    }



    }
