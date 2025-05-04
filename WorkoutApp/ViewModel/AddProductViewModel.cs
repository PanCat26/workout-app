using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WorkoutApp.Models;
using WorkoutApp.Service;

namespace WorkoutApp.ViewModel
{
    public class AddProductViewModel : INotifyPropertyChanged
    {
        private readonly ProductService productService;

        public AddProductViewModel(ProductService productService)
        {
            this.productService = productService;
            Categories = new ObservableCollection<Category>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Properties for binding
        public string Name { get; set; }
        public string Price { get; set; }
        public string Stock { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string PhotoURL { get; set; }
        public Category SelectedCategory { get; set; }

        public ObservableCollection<Category> Categories { get; }

        private string _validationMessage;
        public string ValidationMessage
        {
            get => _validationMessage;
            set => SetProperty(ref _validationMessage, value); // if using ObservableObject from CommunityToolkit.Mvvm
        }


        public async Task LoadCategoriesAsync(CategoryService categoryService)
        {
            var categories = await categoryService.GetAllAsync();
            Categories.Clear();
            foreach (var category in categories)
                Categories.Add(category);
        }

        public async Task<bool> AddProductAsync()
        {
            if (!IsValid(out string validationMessage))
            {
                ValidationMessage = validationMessage;
                return false;
            }

            ValidationMessage = string.Empty; // clear previous errors

            // Safe parsing
            if (!decimal.TryParse(this.Price, out decimal parsedPrice))
            {
                ValidationMessage = "Price must be a valid decimal number.";
                return false;
            }

            if (!int.TryParse(this.Stock, out int parsedStock))
            {
                ValidationMessage = "Stock must be a valid integer.";
                return false;
            }

            var newProduct = new Product(
                id: null,
                name: this.Name,
                price: parsedPrice,
                stock: parsedStock,
                category: this.SelectedCategory,
                size: this.Size,
                color: this.Color,
                description: this.Description,
                photoURL: this.PhotoURL
            );

            try
            {
                var createdProduct = await productService.CreateAsync(newProduct);
                System.Diagnostics.Debug.WriteLine($"[AddProductViewModel] Product created with ID: {createdProduct.ID}");
                return true;
            }
            catch (Exception ex)
            {
                ValidationMessage = $"Error creating product: {ex.Message}";
                return false;
            }
        }

        private bool IsValid(out string error)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                error = "Name is required.";
                return false;
            }

            if (!decimal.TryParse(Price, out decimal parsedPrice) || parsedPrice < 0)
            {
                error = "Price must be a valid positive number.";
                return false;
            }

            if (!int.TryParse(Stock, out int parsedStock) || parsedStock < 0)
            {
                error = "Stock must be a valid positive integer.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Size))
            {
                error = "Size is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Color))
            {
                error = "Color is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                error = "Description is required.";
                return false;
            }

            if (SelectedCategory == null)
            {
                error = "Please select a category.";
                return false;
            }

            error = null;
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}
