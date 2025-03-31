using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;

namespace WorkoutApp.ViewModel
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        public string ProductName {  get; set; }
        public double Price {  get; set; }
        public string ImageSource {  get; set; }
        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

        public CartItemViewModel() { }  
        public CartItemViewModel(string ProductName, string ImageSource, double Price, int Quantity) { 
            this.ProductName = ProductName;
            this.Price = Price;
            this.ImageSource = ImageSource;
            this.Quantity = Quantity;
            TotalPrice = this.Price * this.Quantity;

        }

        public CartItemViewModel(Product product)
        {
            ProductName = product.Name;
            ImageSource = product.Image;
            Quantity = product.Quantity;
            Price = product.Price;
            TotalPrice = Price * Quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
