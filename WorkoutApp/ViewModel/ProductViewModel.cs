// <copyright file="ProductViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.ViewModel // Using the singular 'ViewModel' namespace as per your structure
{
    using System;
    using System.Collections.Generic; // Required for EqualityComparer
    using System.Collections.ObjectModel; // Required for ObservableCollection
    using System.ComponentModel; // Required for INotifyPropertyChanged
    using System.Diagnostics; // Required for Debug.WriteLine
    using System.Globalization; // Required for CultureInfo
    using System.Linq; // Required for .ToList()
    using System.Runtime.CompilerServices; // Required for CallerMemberName
    using System.Threading.Tasks;
    using System.Windows.Input; // Required for ICommand
    using WorkoutApp.Models; // Assuming Product and Category models are here
    using WorkoutApp.Service; // Assuming ProductService and IService<Product> are here
    using WorkoutApp.Utils.Filters; // Required for ProductFilter
    using WorkoutApp.Data.Database; // Required for DbConnectionFactory, DbService
    using WorkoutApp.Repository; // Required for ProductRepository
    using System.Configuration; // Required for ConfigurationManager

    /// <summary>
    /// ViewModel for a single product, designed for UI data binding.
    /// Supports loading, displaying, updating, and deleting a product using Commands.
    /// </summary>
    public class ProductViewModel : INotifyPropertyChanged // Implement INotifyPropertyChanged for UI updates
    {
        // The type is ProductService because GetFilteredAsync is not in IService<Product>
        private readonly ProductService productService;
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

        // New property for related products
        private ObservableCollection<Product> relatedProducts = new ObservableCollection<Product>();

        // Property to control UI editing state
        private bool isEditing = false;

        // Commands for UI Interaction
        public ICommand EnterEditModeCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand DeleteCommand { get; } // The command for the delete button

        // Event required by INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class.
        /// This is the parameterless constructor required for XAML object element usage.
        /// It initializes dependencies with default/placeholder values.
        /// The ViewModel should be properly initialized with data using LoadProductAsync later.
        /// </summary>
        public ProductViewModel()
        {
            Debug.WriteLine("ProductViewModel parameterless constructor called.");
            // Initialize dependencies with default/placeholder values.
            // In a real application, this constructor might be used by a DI container
            // and dependencies would be injected. For this example, we'll new them up.
            // Note: This might not be the ideal way to handle dependencies in a production app,
            // but it satisfies the XAML requirement for a parameterless constructor.
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var connectionFactory = new DbConnectionFactory(connectionString);
            var dbService = new DbService(connectionFactory);
            var productRepository = new ProductRepository(dbService);
            this.productService = new ProductService(productRepository);

            // Initialize Commands
            EnterEditModeCommand = new RelayCommand(ExecuteEnterEditMode);
            SaveCommand = new RelayCommand(async _ => await ExecuteSaveAsync());
            CancelEditCommand = new RelayCommand(async _ => await ExecuteCancelEditAsync());
            DeleteCommand = new RelayCommand(async _ => await ExecuteDeleteAsync());
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class with a product service.
        /// This constructor is typically used when the ViewModel is created programmatically,
        /// allowing for dependency injection of the service.
        /// </summary>
        /// <param name="productService">The product service to fetch product data.</param>
        public ProductViewModel(ProductService productService)
        {
            Debug.WriteLine("ProductViewModel constructor with ProductService called.");
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            // Initial values are set, data will be loaded when LoadProductAsync is called

            // Initialize Commands
            EnterEditModeCommand = new RelayCommand(ExecuteEnterEditMode);
            SaveCommand = new RelayCommand(async _ => await ExecuteSaveAsync());
            CancelEditCommand = new RelayCommand(async _ => await ExecuteCancelEditAsync());
            DeleteCommand = new RelayCommand(async _ => await ExecuteDeleteAsync()); // Initialize the DeleteCommand
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

        /// <summary>
        /// Gets the collection of related products for UI binding.
        /// </summary>
        public ObservableCollection<Product> RelatedProducts
        {
            get => relatedProducts;
            private set => SetProperty(ref relatedProducts, value); // Use SetProperty to notify UI
        }

        /// <summary>
        /// Gets or sets a value indicating whether the product details are currently being edited.
        /// </summary>
        public bool IsEditing
        {
            get => isEditing;
            set => SetProperty(ref isEditing, value); // Notify UI when editing state changes
        }


        // --- Methods for UI Interaction (Executed by Commands) ---

        /// <summary>
        /// Loads the product data asynchronously from the service based on the provided ID.
        /// This method is called after the ViewModel is created, typically from the UI's navigation event.
        /// </summary>
        /// <param name="id">The ID of the product to load.</param>
        public async Task LoadProductAsync(int id)
        {
            this.productId = id; // Store the product ID
            IsEditing = false; // Ensure not in editing mode when loading

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

                    // Load related products after the main product is loaded
                    if (product.Category != null)
                    {
                        // Call the new generic GetFilteredAsync method
                        await LoadRelatedProductsAsync(product.Category.ID ?? 0, product.ID.Value, 3); // Get 3 related products
                    }
                    else
                    {
                        // Handle case where product has no category
                        RelatedProducts.Clear(); // Clear related products if no category
                    }
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
                    RelatedProducts.Clear(); // Clear related products if main product not found
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
                RelatedProducts.Clear(); // Clear related products on error
            }
        }

        /// <summary>
        /// Loads related products asynchronously based on category and exclusion.
        /// </summary>
        /// <param name="categoryId">The category ID to filter by.</param>
        /// <param name="excludeProductId">The ID of the product to exclude.</param>
        /// <param name="count">The number of related products to fetch.</param>
        private async Task LoadRelatedProductsAsync(int categoryId, int excludeProductId, int count)
        {
            try
            {
                // Create a ProductFilter instance with the desired criteria
                var filter = new ProductFilter(categoryId, excludeProductId, count);

                // Call the new generic service method to get filtered products
                var related = await productService.GetFilteredAsync(filter);

                // Clear existing related products and add the new ones
                RelatedProducts.Clear();
                foreach (var p in related)
                {
                    RelatedProducts.Add(p);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading related products for category {categoryId}: {ex}");
                RelatedProducts.Clear(); // Clear related products on error
            }
        }

        /// <summary>
        /// Executes the command to set the ViewModel to editing mode.
        /// </summary>
        private void ExecuteEnterEditMode(object? parameter)
        {
            Debug.WriteLine("EnterEditModeCommand executed."); // Added logging
            IsEditing = true;
        }

        /// <summary>
        /// Executes the command to save the changes to the product.
        /// </summary>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        private async Task ExecuteSaveAsync()
        {
            Debug.WriteLine("SaveCommand executed. Attempting to save product."); // Added logging
            if (product == null || product.ID == null)
            {
                Debug.WriteLine("Attempted to save a product that was not loaded correctly."); // Added logging
                IsEditing = false; // Exit editing mode
                return;
            }

            // Create an updated Product model from the ViewModel properties
            var updatedProduct = new Product(
                id: product.ID, // Use the existing ID
                name: Name,
                price: Price,
                stock: Stock,
                // Need to create a Category object from ViewModel properties
                // Assuming CategoryName is just for display and CategoryID is used for saving
                category: new Category(CategoryID, CategoryName), // Pass both ID and Name
                size: Size,
                color: Color,
                description: Description,
                photoURL: PhotoURL
            );

            try
            {
                // Call the service to update the product
                // Assume ProductService.UpdateAsync exists and takes a Product object
                // It should return the updated product from the database
                Product resultProduct = await productService.UpdateAsync(updatedProduct);

                // Update the underlying product model in the ViewModel
                product = resultProduct;

                // Update ViewModel properties from the result in case the service modified them (e.g., calculated fields)
                Name = product.Name;
                Price = product.Price;
                Stock = product.Stock;
                CategoryID = product.Category?.ID ?? 0;
                CategoryName = product.Category?.Name ?? "Unknown Category";
                Size = product.Size;
                Color = product.Color;
                Description = product.Description;
                PhotoURL = product.PhotoURL;


                Debug.WriteLine($"Product ID {product.ID} updated successfully."); // Added logging

                IsEditing = false; // Exit editing mode on success
                // Optionally, show a success message to the user
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving product ID {product.ID}: {ex}"); // Added logging
                // Handle the error (e.g., show an error message in the UI)
                // Keep IsEditing as true to allow the user to correct issues or cancel
                // Optionally, revert ViewModel properties to original values
                // For now, we'll leave the entered values in the UI
            }
        }

        /// <summary>
        /// Executes the command to cancel the current editing session and reverts ViewModel properties.
        /// </summary>
        /// <returns>A Task representing the asynchronous cancel operation.</returns>
        private async Task ExecuteCancelEditAsync()
        {
            Debug.WriteLine("CancelEditCommand executed. Reverting changes."); // Added logging
            // Re-load the product data from the service to discard changes
            // This is one way to reset; another is to store original values
            // when entering edit mode. Re-loading is simpler here.
            if (product != null)
            {
                // Re-load the currently displayed product
                await LoadProductAsync(product.ID.Value);
            }
            else
            {
                // If product was never loaded successfully, just exit editing
                IsEditing = false;
            }

            Debug.WriteLine($"Editing cancelled for product ID {productId}."); // Added logging
        }

        /// <summary>
        /// Executes the command to delete the product.
        /// Made public to be accessible from XAML x:Bind.
        /// </summary>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        public async Task ExecuteDeleteAsync() // Made public
        {
            Debug.WriteLine("DeleteCommand executed. Attempting to delete product."); // Added logging
            if (product == null || product.ID == null)
            {
                Debug.WriteLine("Attempted to delete a product that was not loaded correctly."); // Added logging
                return; // Command finished, no action taken
            }

            // --- Placeholder for Confirmation Dialog ---
            // In a real app, you would show a confirmation dialog here.
            // For this example, we'll assume the user confirmed.
            Debug.WriteLine($"Attempting to delete product ID {product.ID}. (Assuming user confirmation)"); // Added logging
            // bool confirmed = await ShowConfirmationDialogAsync($"Are you sure you want to delete {Name}?");
            // if (!confirmed) return;
            // --- End Placeholder ---


            try
            {
                // Call the service to delete the product
                // Assume ProductService.DeleteAsync exists and takes a product ID
                Debug.WriteLine($"Calling productService.DeleteAsync({product.ID.Value})..."); // Added logging
                bool success = await productService.DeleteAsync(product.ID.Value);
                Debug.WriteLine($"productService.DeleteAsync returned: {success}"); // Added logging

                if (success)
                {
                    Debug.WriteLine($"Product ID {product.ID} deleted successfully."); // Added logging

                    // --- Update UI State After Successful Deletion ---
                    // Clear ViewModel properties to visually indicate deletion
                    Name = "Product Deleted";
                    Price = 0;
                    Stock = 0;
                    CategoryID = 0;
                    CategoryName = "";
                    Size = "";
                    Color = "";
                    Description = "This product has been deleted.";
                    PhotoURL = null; // Clear the image
                    RelatedProducts.Clear(); // Clear related products
                    IsEditing = false; // Ensure not in editing mode

                    Debug.WriteLine("ViewModel properties updated after successful deletion."); // Added logging

                    // In a real application, you would typically raise an event here
                    // (e.g., OnProductDeleted event) that the View (ProductDetailPage.xaml.cs)
                    // listens to and then handles navigation away from the page.
                    // For example: OnProductDeleted?.Invoke(this, EventArgs.Empty);
                    // --- End UI Update ---

                }
                else
                {
                    Debug.WriteLine($"Failed to delete product ID {product.ID}. Service reported failure."); // Added logging
                    // Handle the error (e.g., show an error message in the UI)
                    // Optionally, show a message like "Deletion Failed"
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting product ID {product.ID}: {ex}"); // Added logging
                // Handle the error (e.g., show an error message in the UI)
                // Optionally, show an error message like "An error occurred during deletion."
            }
            Debug.WriteLine("ExecuteDeleteAsync finished."); // Added logging
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


        // --- Basic ICommand Implementation (RelayCommand) ---
        // In a real application, this would be in a separate utility file/namespace.
        private class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Func<object?, bool>? _canExecute;

            public event EventHandler? CanExecuteChanged;

            public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }

            public void Execute(object? parameter)
            {
                Debug.WriteLine($"RelayCommand Execute called for command: {_execute.Method.Name}"); // Added logging
                _execute(parameter);
            }

            // Method to manually raise CanExecuteChanged (if needed for dynamic enabling/disabling)
            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
