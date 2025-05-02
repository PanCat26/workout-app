using System.ComponentModel;

namespace WorkoutApp.ViewModel
{
    public class ProductDetailsViewModel : INotifyPropertyChanged
    {
        /*
        private readonly ProductService _productService;
        private readonly WishlistService _wishlistService;
        private readonly CartService _cartService;

        // private MOCKProduct product = new MOCKProduct("Sports Bra", "$29.99", "https://i.pinimg.com/736x/f9/15/2b/f9152bb652ed2ef22f4271752e9ffbc0.jpg", "A comfortable sports bra", "XS, S, M, L", "White, Black, Red, Blue, Green", "10");
        private IProduct product;


        public int ProductID => product.ID;
        public string ProductName => product.Name;
        public string Price => "$" + product.Price.ToString("F2");
        public string ImageSource => product.FileUrl;
        public int Stock => product.Stock;
        public string Description => product.Description;



        public ObservableCollection<string> AvailableSizes { get; set; }
        private string _selectedSize;

        public ObservableCollection<string> AvailableQuantities { get; set; }
        private string _selectedQuantity;
        public ObservableCollection<IProduct> RecommendedProducts { get; set; }

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
        */

        /*public ProductDetailsViewModel(ProductService productService, WishlistService wishlistService, CartService cartService, IProduct product)
        {

            _productService = productService;
            _wishlistService = wishlistService;
            _cartService = cartService;
            this.product = product;
            this.AvailableColors = new ObservableCollection<string>();
            this.AvailableSizes = new ObservableCollection<string>();
            this.AvailableQuantities = new ObservableCollection<string>();
            this.RecommendedProducts = new ObservableCollection<IProduct>();
            LoadProductDetails();



        }*/

        private void LoadProductDetails()
        {/*
            if (product is ClothesProduct clothesProduct)
            {
                LoadColors(clothesProduct.Attributes);
                LoadSizes(clothesProduct.Size);
            }
            if (product is FoodProduct foodProduct)
            {
                LoadSizes(foodProduct.Size);
            }
            LoadQuantities();
            LoadRecommendedProducts();*/
        }

        private void LoadColors(string colorData)
        {
            /*
            AvailableColors.Clear();
            var colors = colorData.Split(',').Select(c => c.Trim()).ToList();
            foreach (var color in colors)
            {
                AvailableColors.Add(color);
            }*/
        }

        private void LoadSizes(string sizeData)
        {
            /*
            AvailableSizes.Clear();
            var sizes = sizeData.Split(',').Select(s => s.Trim()).ToList();
            foreach (var size in sizes)
            {
                AvailableSizes.Add(size);
            }*/
        }

        private void LoadQuantities()
        {
            /*
            AvailableQuantities.Clear();
            for (int i = 1; i <= product.Stock; i++)
            {
                AvailableQuantities.Add(i.ToString());
            }*/
        }

        /*private async Task LoadRecommendedProducts()
        {
            /*
            RecommendedProducts.Clear();
            var similarProducts = await _productService.GetRecommendedProductsAsync(product.ID);
            foreach (var product in similarProducts)
            {
                RecommendedProducts.Add(product);
            }
            
        }*/

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



    }
}

public class MOCKProduct
{
    public MOCKProduct(string productName, string price, string imageSource, string description, string sizes, string colors, string quantity)
    {
        ProductName = productName;
        Price = price;
        ImageSource = imageSource;
        Description = description;
        Sizes = sizes;
        Colors = colors;
        Quantity = quantity;


    }
    public MOCKProduct()
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