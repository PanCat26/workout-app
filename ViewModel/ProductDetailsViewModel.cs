using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace WorkoutApp.ViewModel
{


    public class ProductDetailsViewModel : INotifyPropertyChanged
    {

        private Product product = new Product("Sports Bra", "$29.99", "https://i.pinimg.com/736x/f9/15/2b/f9152bb652ed2ef22f4271752e9ffbc0.jpg", "A comfortable sports bra", "XS, S, M, L", "White, Black, Red, Blue, Green", "10");
        public string ProductName => product.ProductName;
        public string Price => product.Price;
        public string ImageSource => product.ImageSource;
        public string Description => product.Description;

        

        public ObservableCollection<string> AvailableSizes { get; set; }
        private string _selectedSize;

        public ObservableCollection<string> AvailableQuantities { get; set; }
        private string _selectedQuantity;
        public ObservableCollection<Product> RecommendedProducts { get; set; }

        public ObservableCollection<string> AvailableColors { get; set; }

        private string _selectedColor = "Choose Color";
        public string SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
            }
        }

        public string SelectedSize
        {
            get => _selectedSize;
            set
            {
                if (_selectedSize != value)
                {
                    _selectedSize = value;
                    OnPropertyChanged(nameof(SelectedSize));
                }
            }
        }
        public string SelectedQuantity
        {
            get => _selectedQuantity;
            set
            {
                if (_selectedQuantity != value)
                {
                    _selectedQuantity = value;
                    OnPropertyChanged(nameof(SelectedQuantity));
                }
            }
        }
        public ICommand SelectColorCommand { get; }


        public ProductDetailsViewModel()
        {
            LoadColors();
            LoadSizes();
            LoadQuantities();
            RecommendedProducts = new ObservableCollection<Product>
            {
                new Product { ProductName = "Sports Bra", Price = "$29.99", ImageSource = "/Assets/sportsBra.jpg" },
                new Product { ProductName = "Logo T-shirt", Price = "$19.99", ImageSource = "https://i.pinimg.com/736x/cd/0f/f5/cd0ff5c42be631ecaaa065d69c84255b.jpg" },
                new Product { ProductName = "Running Shoes", Price = "$79.99", ImageSource = "https://i.pinimg.com/736x/e7/20/f3/e720f38675e34aca5325c084a4d7efa6.jpg" }
            };
        }
    


        private void LoadColors()
        {
            // Simulating fetching colors from a database

            AvailableColors = new ObservableCollection<string>();
            List<string> colors= product.Colors.Split(",").ToList();
            for (int i = 0; i < colors.Count; i++)
            {
                AvailableColors.Add(colors[i]);
            }
        }

        private void LoadSizes()
        {
            // Simulating fetching sizes from a database
            AvailableSizes = new ObservableCollection<string>();
            List<string> sizes = product.Sizes.Split(",").ToList();
            for (int i = 0; i < sizes.Count; i++)
            {
                AvailableSizes.Add(sizes[i]);
            }
        }

        private void LoadQuantities()
        {
            // Simulating fetching quantities from a database
            AvailableQuantities = new ObservableCollection<string>();
            int quantity = Convert.ToInt32(product.Quantity);
            for (int i = 1; i <= quantity; i++)
            {
                AvailableQuantities.Add(i.ToString());
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



    }

  

}

public class Product
{
    public Product(string productName, string price, string imageSource, string description, string sizes, string colors, string quantity  )
    {
        ProductName = productName;
        Price = price;
        ImageSource = imageSource;
        Description = description;
        Sizes = sizes;
        Colors = colors;
        Quantity = quantity;


    }
    public Product()
    {
    }

    public string ProductName { get; set; }
    public string Price { get; set; }
    public string ImageSource { get; set; }
    public string Description { get; set; }
    public string Sizes { get; set; }
    public string Colors { get; set; }
    public string Quantity { get; set; }

}





