// <copyright file="ProductViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.ViewModel // Using the singular 'ViewModel' namespace as per your structure
{
    using System;
    using System.Collections.Generic; // Required for EqualityComparer
    using System.ComponentModel; // Required for INotifyPropertyChanged
    using System.Diagnostics; // Required for Debug.WriteLine
    using System.Globalization; // Required for CultureInfo
    using System.Runtime.CompilerServices; // Required for CallerMemberName
    using System.Threading.Tasks;
    using WorkoutApp.Models; // Assuming Product and Category models are here
    using WorkoutApp.Service; // Assuming ProductService and IService<Product> are here

    /// <summary>
    /// ViewModel for a single product, designed for UI data binding.
    /// </summary>
    public class ProductViewModel : INotifyPropertyChanged // Implement INotifyPropertyChanged for UI updates
    {
        private readonly IService<Product> productService;
        private int productId; // Store the product ID internally once loaded
        private Product? product; // Hold the underlying Product model

        // Properties to expose Product data for binding
        private string name = "Loading...";
        private decimal price = 0.00m;
        private int stock = 0;
        private int categoryId = 0; // ViewModel property to hold Category ID
        private string categoryName = "Loading..."; // ViewModel property to hold Category Name
        private string size = "N/A";
        private string color = "N/A";
        private string description = "Loading description...";
        private string? photoUrl = null; // Use string? for nullable PhotoURL

        // Event required by INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class.
        /// </summary>
        /// <param name="productService">The product service to fetch product data.</param>
        // CORRECTED CONSTRUCTOR: Takes only the service dependency.
        // The product ID is passed to LoadProductAsync after the ViewModel is created.
        public ProductViewModel(IService<Product> productService)
        {
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            // Initial values are set, data will be loaded when LoadProductAsync is called
        }

        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
        // This property doesn't change after loading, so no need for OnPropertyChanged
        public int ID => product?.ID ?? productId; // Return loaded ID or initial ID if not loaded

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value); // Use SetProperty to notify UI on change
        }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price
        {
            get => price;
            set
            {
                if (SetProperty(ref price, value))
                {
                    OnPropertyChanged(nameof(FormattedPrice)); // Notify UI when Price changes
                }
            }
        }

        /// <summary>
        /// Gets the formatted price as a currency string.
        /// This property is used for UI binding since StringFormat is not supported in WinUI XAML.
        /// </summary>
        public string FormattedPrice => Price.ToString("C2", CultureInfo.CurrentCulture);


        /// <summary>
        /// Gets or sets the stock quantity of the product.
        /// </summary>
        public int Stock
        {
            get => stock;
            set => SetProperty(ref stock, value);
        }

        /// <summary>
        /// Gets or sets the category ID of the product.
        /// </summary>
        public int CategoryID
        {
            get => categoryId;
            set => SetProperty(ref categoryId, value);
        }

        /// <summary>
        /// Gets or sets the category name of the product.
        /// </summary>
        public string CategoryName
        {
            get => categoryName;
            set => SetProperty(ref categoryName, value);
        }


        /// <summary>
        /// Gets or sets the size of the product.
        /// </summary>
        public string Size
        {
            get => size;
            set => SetProperty(ref size, value);
        }

        /// <summary>
        /// Gets or sets the color of the product.
        /// </summary>
        public string Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        /// <summary>
        /// Gets or sets the URL of the product photo.
        /// </summary>
        public string? PhotoURL
        {
            get => photoUrl;
            set => SetProperty(ref photoUrl, value);
        }

        // --- Commands for UI Interaction (Placeholders) ---
        // In a real app, you'd use ICommand implementations (e.g., RelayCommand, AsyncCommand)
        // and bind UI buttons to these properties.

        // public ICommand AddToCartCommand { get; }
        // public ICommand AddToWishlistCommand { get; }
        // public ICommand EditProductCommand { get; }
        // public ICommand DeleteProductCommand { get; }

        /// <summary>
        /// Loads the product data asynchronously from the service based on the provided ID.
        /// This method is called after the ViewModel is created, typically from the UI's navigation event.
        /// </summary>
        /// <param name="id">The ID of the product to load.</param>
        public async Task LoadProductAsync(int id)
        {
            this.productId = id; // Store the product ID

            try
            {
                // Use the injected service to get the product by ID
                product = await productService.GetByIdAsync(productId);

                if (product != null)
                {
                    // Update ViewModel properties based on the loaded Product model
                    // SetProperty will raise PropertyChanged event for UI updates
                    Name = product.Name;
                    Price = product.Price; // Setting Price will also update FormattedPrice
                    Stock = product.Stock;
                    // Access CategoryID from the Category object within the Product model
                    CategoryID = product.Category?.ID ?? 0; // Use null conditional operator in case Category is null
                    CategoryName = product.Category?.Name ?? "Unknown Category"; // Also add Category Name
                    Size = product.Size;
                    Color = product.Color;
                    Description = product.Description;
                    PhotoURL = product.PhotoURL;
                    // ID is already set or derived
                }
                else
                {
                    // Handle case where product is not found
                    Name = "Product Not Found";
                    Description = $"Product with ID {productId} could not be loaded.";
                    // Reset other properties or show default values
                    Price = 0; // Setting Price will also update FormattedPrice
                    Stock = 0;
                    CategoryID = 0;
                    CategoryName = "N/A";
                    Size = "N/A";
                    Color = "N/A";
                    PhotoURL = null;
                }
            }
            catch (Exception ex)
            {
                // Handle loading errors (e.g., log the error, show an error message in the UI)
                Name = "Error Loading Product";
                Description = $"Failed to load product with ID {productId}. Error: {ex.Message}";
                Debug.WriteLine($"Error loading product {productId}: {ex}");
                // Reset other properties or show default values
                Price = 0; // Setting Price will also update FormattedPrice
                Stock = 0;
                CategoryID = 0;
                CategoryName = "N/A";
                Size = "N/A";
                Color = "N/A";
                PhotoURL = null;
            }
        }


        /// <summary>
        /// Helper method to set property value and raise PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The reference to the backing field.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="propertyName">The name of the property (automatically inferred).</param>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false; // Value hasn't changed
            }

            field = value; // Update the backing field
            OnPropertyChanged(propertyName); // Notify the UI
            return true; // Value was changed
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
