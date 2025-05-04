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

        // Commands for UI Interaction
        // These are primarily for the modal's buttons
        public ICommand? SaveCommand { get; }
        public ICommand? CancelEditCommand { get; }
        public ICommand? DeleteCommand { get; } // The command for the delete button

        // Property to track if the update modal is currently open (useful for ViewModel state)
        private bool isUpdateModalOpen = false;
        public bool IsUpdateModalOpen
        {
            get => isUpdateModalOpen;
            set => SetProperty(ref isUpdateModalOpen, value); // Use SetProperty to notify UI when modal state changes
        }

        // New event to signal the View to show the update modal
        public event EventHandler? RequestShowUpdateModal;


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
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var connectionFactory = new DbConnectionFactory(connectionString);
            var dbService = new DbService(connectionFactory);
            var productRepository = new ProductRepository(dbService);
            this.productService = new ProductService(productRepository);

            // Initialize Commands
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
            SaveCommand = new RelayCommand(async _ => await ExecuteSaveAsync());
            CancelEditCommand = new RelayCommand(async _ => await ExecuteCancelEditAsync());
            DeleteCommand = new RelayCommand(async _ => await ExecuteDeleteAsync()); // Initialize the DeleteCommand
        }

        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
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


        // --- Methods for UI Interaction (Executed by Commands or x:Bind) ---

        /// <summary>
        /// Loads the product data asynchronously from the service based on the provided ID.
        /// This method is called after the ViewModel is created, typically from the UI's navigation event.
        /// </summary>
        /// <param name="id">The ID of the product to load.</param>
        public async Task LoadProductAsync(int id)
        {
            Debug.WriteLine($"ProductViewModel: LoadProductAsync called with ID: {id}"); // Added logging
            this.productId = id; // Store the product ID
            IsUpdateModalOpen = false; // Ensure modal is closed when loading a new product

            try
            {
                // Use the injected service to get the product by ID
                product = await productService.GetByIdAsync(productId);

                if (product != null)
                {
                    Debug.WriteLine($"ProductViewModel: Product ID {id} loaded successfully."); // Added logging
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

                    Debug.WriteLine($"ProductViewModel: Properties updated after loading: Name={Name}, Price={Price}, Stock={Stock}, CategoryID={CategoryID}, CategoryName={CategoryName}"); // Added logging

                    // Load related products after the main product is loaded
                    if (product.Category != null)
                    {
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
                    Debug.WriteLine($"ProductViewModel: Product ID {id} not found."); // Added logging
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
                Debug.WriteLine($"ProductViewModel: Error loading product {productId}: {ex}"); // Added logging
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
            Debug.WriteLine($"ProductViewModel: LoadProductAsync finished for ID: {id}"); // Added logging
        }

        /// <summary>
        /// Loads related products asynchronously based on category and exclusion.
        /// </summary>
        /// <param name="categoryId">The category ID to filter by.</param>
        /// <param name="excludeProductId">The ID of the product to exclude.</param>
        /// <param name="count">The number of related products to fetch.</param>
        private async Task LoadRelatedProductsAsync(int categoryId, int excludeProductId, int count)
        {
            Debug.WriteLine($"ProductViewModel: LoadRelatedProductsAsync called for Category ID: {categoryId}, Exclude ID: {excludeProductId}, Count: {count}"); // Added logging
            try
            {
                // Create a ProductFilter instance with the desired criteria
                var filter = new ProductFilter(categoryId, excludeProductId, count, null, null, null);

                // Call the new generic service method to get filtered products
                var related = await productService.GetFilteredAsync(filter);

                // Clear existing related products and add the new ones
                RelatedProducts.Clear();
                foreach (var p in related)
                {
                    RelatedProducts.Add(p);
                }
                Debug.WriteLine($"ProductViewModel: Loaded {RelatedProducts.Count} related products."); // Added logging
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ProductViewModel: Error loading related products for category {categoryId}: {ex}"); // Added logging
                RelatedProducts.Clear(); // Clear related products on error
            }
        }


        /// <summary>
        /// Executes the method to prepare for editing and signals the View to show the modal.
        /// Made public to be accessible from XAML x:Bind.
        /// </summary>
        public async Task ExecuteEnterEditModeAsync() // Made public and async for x:Bind
        {
            Debug.WriteLine("ProductViewModel: ExecuteEnterEditModeAsync called."); // Added logging
            // Ensure the product is loaded before attempting to edit
            if (product == null && ID > 0)
            {
                Debug.WriteLine("ProductViewModel: Product is null, attempting to load product before entering edit mode.");
                await LoadProductAsync(ID); // Load if not already loaded
            }

            if (product != null)
            {
                // Ensure ViewModel properties are up-to-date with the 'product' model before showing the modal
                // Although LoadProductAsync does this, explicitly doing it here again ensures the latest data
                // is in the bindable properties right before the modal is requested.
                Name = product.Name;
                Price = product.Price;
                Stock = product.Stock;
                CategoryID = product.Category?.ID ?? 0;
                CategoryName = product.Category?.Name ?? "Unknown Category";
                Size = product.Size;
                Color = product.Color;
                Description = product.Description;
                PhotoURL = product.PhotoURL;
                Debug.WriteLine($"ProductViewModel: Properties confirmed before opening modal: Name={Name}, Price={Price}, Stock={Stock}, CategoryID={CategoryID}, CategoryName={CategoryName}"); // Added logging


                IsUpdateModalOpen = true; // Set ViewModel state
                Debug.WriteLine("ProductViewModel: IsUpdateModalOpen set to true. Raising RequestShowUpdateModal event."); // Added logging
                                                                                                                           // Raise the event to signal the View to show the modal
                RequestShowUpdateModal?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Debug.WriteLine("ProductViewModel: Cannot enter edit mode, product is null after load attempt."); // Added logging
                                                                                                                  // Optionally, show an error message to the user
            }
        }

        /// <summary>
        /// Executes the command/method to save the changes to the product.
        /// Called from the modal's Save button.
        /// Made public to be accessible from XAML x:Bind.
        /// </summary>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        public async Task ExecuteSaveAsync() // Made public
        {
            Debug.WriteLine("ProductViewModel: ExecuteSaveAsync called. Attempting to save product."); // Added logging
            if (product == null || product.ID == null)
            {
                Debug.WriteLine("ProductViewModel: Attempted to save a product that was not loaded correctly."); // Added logging
                IsUpdateModalOpen = false; // Close the modal
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

            Debug.WriteLine($"ProductViewModel: Attempting to save with values: Name={updatedProduct.Name}, Price={updatedProduct.Price}, Stock={updatedProduct.Stock}, CategoryID={updatedProduct.Category?.ID}"); // Added logging

            try
            {
                // Call the service to update the product
                Debug.WriteLine($"ProductViewModel: Calling productService.UpdateAsync({product.ID.Value})..."); // Added logging
                Product resultProduct = await productService.UpdateAsync(updatedProduct);
                Debug.WriteLine($"ProductViewModel: productService.UpdateAsync returned."); // Added logging


                // Update the underlying product model in the ViewModel
                product = resultProduct;

                // Update ViewModel properties from the result in case the service modified them (e.g., calculated fields)
                // These properties are already bound to the UI, so updating them will refresh the display
                Name = product.Name;
                Price = product.Price;
                Stock = product.Stock;
                CategoryID = product.Category?.ID ?? 0;
                CategoryName = product.Category?.Name ?? "Unknown Category";
                Size = product.Size;
                Color = product.Color;
                Description = product.Description;
                PhotoURL = product.PhotoURL;

                Debug.WriteLine($"ProductViewModel: Properties updated after save: Name={Name}, Price={Price}, Stock={Stock}, CategoryID={CategoryID}, CategoryName={CategoryName}"); // Added logging


                Debug.WriteLine($"ProductViewModel: Product ID {product.ID} updated successfully."); // Added logging

                IsUpdateModalOpen = false; // Close the modal on success
                Debug.WriteLine("ProductViewModel: IsUpdateModalOpen set to false."); // Added logging
                // Optionally, show a success message to the user (e.g., using a InfoBar)
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ProductViewModel: Error saving product ID {product.ID}: {ex}"); // Added logging
                // Handle the error (e.g., show an error message in the UI, keep modal open)
                // IsUpdateModalOpen remains true so the user can fix the error or cancel
            }
            Debug.WriteLine("ProductViewModel: ExecuteSaveAsync finished."); // Added logging
        }

        /// <summary>
        /// Executes the command/method to cancel the current editing session and reverts ViewModel properties.
        /// Called from the modal's Cancel button.
        /// Made public to be accessible from XAML x:Bind.
        /// </summary>
        /// <returns>A Task representing the asynchronous cancel operation.</returns>
        public async Task ExecuteCancelEditAsync() // Made public
        {
            Debug.WriteLine("ProductViewModel: ExecuteCancelEditAsync called. Reverting changes."); // Added logging
            // Re-load the product data from the service to discard changes
            if (product != null)
            {
                await LoadProductAsync(product.ID.Value); // This will reset all ViewModel properties
                Debug.WriteLine($"ProductViewModel: Properties reverted after cancel: Name={Name}, Price={Price}, Stock={Stock}, CategoryID={CategoryID}, CategoryName={CategoryName}"); // Added logging
            }
            else
            {
                // If product was never loaded successfully, just exit editing
                // Reset ViewModel properties to default state
                Name = "Product Not Loaded";
                Price = 0;
                Stock = 0;
                CategoryID = 0;
                CategoryName = "N/A";
                Size = "N/A";
                Color = "N/A";
                Description = "";
                PhotoURL = null;
                RelatedProducts.Clear();
                Debug.WriteLine("ProductViewModel: Properties reset to default after cancel (product was null)."); // Added logging
            }

            IsUpdateModalOpen = false; // Close the modal
            Debug.WriteLine("ProductViewModel: IsUpdateModalOpen set to false."); // Added logging
            Debug.WriteLine($"ProductViewModel: Editing cancelled for product ID {(product?.ID.HasValue == true ? product.ID.Value : "N/A")}."); // Added logging
        }

        /// <summary>
        /// Executes the command to delete the product.
        /// Made public to be accessible from XAML x:Bind.
        /// </summary>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        public async Task ExecuteDeleteAsync() // Made public
        {
            Debug.WriteLine("ProductViewModel: ExecuteDeleteAsync called."); // Added logging at the very beginning
            if (product == null || product.ID == null)
            {
                Debug.WriteLine("ProductViewModel: Attempted to delete a product that was not loaded correctly (product or ID is null)."); // Added logging
                return; // Command finished, no action taken
            }

            // --- Placeholder for Confirmation Dialog ---
            // In a real app, you would show a confirmation dialog here.
            // For this example, we'll assume the user confirmed.
            Debug.WriteLine($"ProductViewModel: Attempting to delete product ID {product.ID}. (Assuming user confirmation)"); // Added logging
            // bool confirmed = await ShowConfirmationDialogAsync($"Are you sure you want to delete {Name}?");
            // if (!confirmed) return;
            // --- End Placeholder ---


            try
            {
                // Call the service to delete the product
                Debug.WriteLine($"ProductViewModel: Calling productService.DeleteAsync({product.ID.Value})..."); // Added logging
                bool success = await productService.DeleteAsync(product.ID.Value);
                Debug.WriteLine($"ProductViewModel: productService.DeleteAsync returned: {success}"); // Added logging

                if (success)
                {
                    Debug.WriteLine($"ProductViewModel: Product ID {product.ID} deleted successfully."); // Added logging

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
                    IsUpdateModalOpen = false; // Ensure modal is closed if open

                    Debug.WriteLine("ProductViewModel: ViewModel properties updated after successful deletion."); // Added logging

                    // In a real application, you would typically raise an event here
                    // (e.g., OnProductDeleted event) that the View (ProductDetailPage.xaml.cs)
                    // listens to and then handles navigation away from the page.
                    // For example: OnProductDeleted?.Invoke(this, EventArgs.Empty);
                    // --- End UI Update ---

                }
                else
                {
                    Debug.WriteLine($"ProductViewModel: Failed to delete product ID {product.ID}. Service reported failure."); // Added logging
                    // Handle the error (e.g., show an error message in the UI)
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ProductViewModel: Error deleting product ID {product.ID}: {ex}"); // Added logging
                // Handle the error (e.g., show an error message in the UI)
            }
            Debug.WriteLine("ProductViewModel: ExecuteDeleteAsync finished."); // Added logging
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
                // Debug.WriteLine($"ProductViewModel: SetProperty for {propertyName} - Value unchanged."); // Optional: Log if value is the same
                return false; // Value hasn't changed
            }

            // Debug.WriteLine($"ProductViewModel: SetProperty for {propertyName} - Value changing from '{field}' to '{value}'."); // Added logging for property changes
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
            Debug.WriteLine($"ProductViewModel: OnPropertyChanged called for: {propertyName}"); // Added logging
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Product GetSelectedProduct()
        {
            return this.product;
        }

        // --- Basic ICommand Implementation (RelayCommand) ---
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
