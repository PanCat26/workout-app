using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;
using WorkoutApp.Models;
namespace WorkoutApp.ViewModel
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        public string ProductName {  get; set; }
        public string Price {  get; set; }
        public string ImageSource {  get; set; }
        public string Quantity { get; set; }

        public string TotalPrice { get; set; }

        public CartItemViewModel() { }  
        public CartItemViewModel(string ProductName, string ImageSource, string Price, string Quantity) { 
            this.ProductName = ProductName;
            this.Price = Price;
            this.ImageSource = ImageSource;
            this.Quantity = Quantity;
            TotalPrice = "totalPrice";

        }

        public CartItemViewModel(Product product)
        {
            ProductName = product.Name;
            ImageSource = product.Image;
            Quantity = product.Quantity.ToString();
            Price = product.Price.ToString();
            TotalPrice = "totalPrice";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
