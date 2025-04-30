using System.ComponentModel;
namespace WorkoutApp.ViewModel
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        public string ProductName { get; set; }
        public string Price { get; set; }
        public string ImageSource { get; set; }
        public string Quantity { get; set; }

        public string TotalPrice { get; set; }

        public CartItemViewModel() { }
        public CartItemViewModel(string ProductName, string ImageSource, double Price, int Quantity)
        {
            this.ProductName = ProductName;
            this.Price = "$" + string.Format("{0:0.##}", Price);
            this.ImageSource = ImageSource;
            this.Quantity = Quantity.ToString();
            TotalPrice = "$" + string.Format("{0:0.##}", Price * Quantity);

        }
        /*
        public CartItemViewModel(IProduct product)
        {
            ProductName = product.Name;
            ImageSource = product.FileUrl;
            Quantity = product.Stock.ToString();
            Price = product.Price.ToString();
            TotalPrice = "totalPrice";
        }
        */

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
