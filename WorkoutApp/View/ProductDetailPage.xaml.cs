// <copyright file="ProductDetailPage.xaml.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
// </copyright>

namespace WorkoutApp.View // Using the 'View' namespace as in your provided code
{
    using System.Configuration;
    using Microsoft.UI.Xaml.Controls; // For WinUI Page
    using Microsoft.UI.Xaml.Navigation; // For NavigationEventArgs
    using WorkoutApp.Data.Database; // Assuming DbConnectionFactory and DbService are here
    using WorkoutApp.Repository; // Assuming ProductRepository and IRepository are here
    using WorkoutApp.Service; // Assuming ProductService and IService are here
    using WorkoutApp.ViewModel; // Corrected: Using the singular 'ViewModel' namespace for ProductViewModel
    using Microsoft.UI.Xaml; // Required for RoutedEventArgs - USED FOR BUTTON CLICKS
    using System; // Required for System namespace
    using System.Diagnostics; // Required for Debug.WriteLine
    using System.ComponentModel; // Required for PropertyChangedEventArgs

    /// <summary>
    /// Code-behind for the ProductDetailPage.xaml.
    /// </summary>
    public sealed partial class ProductDetailPage : Page // Inherit from Page
    {
        // Public ViewModel property for XAML binding
        public ProductViewModel ViewModel { get; }

        // Private field to store the hosting Window
        private readonly Window hostingWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailPage"/> class.
        /// </summary>
        /// <param name="hostingWindow">The window that is hosting this page.</param>
        // MODIFIED CONSTRUCTOR: Added a parameter to receive the hosting Window
        public ProductDetailPage(Window hostingWindow)
        {
            Debug.WriteLine("ProductDetailPage: Constructor called."); // Added logging
            this.InitializeComponent();

            // Store the reference to the hosting window
            this.hostingWindow = hostingWindow ?? throw new ArgumentNullException(nameof(hostingWindow));

            // Initialize dependencies for the ProductService.
            // In a real application, you would typically use a Dependency Injection container here
            // instead of newing up dependencies directly. This is done here to match
            // the structure of your provided DrinkDetailPage example.
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var connectionFactory = new DbConnectionFactory(connectionString);
            var dbService = new DbService(connectionFactory);
            var productRepository = new ProductRepository(dbService);
            var productService = new ProductService(productRepository);

            // Initialize the ViewModel with the necessary service
            ViewModel = new ProductViewModel(productService);
            Debug.WriteLine($"ProductDetailPage: ViewModel created. Initial ViewModel.ID: {ViewModel.ID}"); // Added logging

            // Set the DataContext of the page to the ViewModel
            this.DataContext = ViewModel;

            // Subscribe to the ViewModel's PropertyChanged event to detect when ID changes
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Debug.WriteLine("ProductDetailPage: Subscribed to ViewModel.PropertyChanged."); // Added logging

            // You can add other initialization logic here if needed,
            // similar to how you set the RemoveButtonText in DrinkDetailPage.
            // For example, logic based on user roles or product status.
        }

        /// <summary>
        /// Handles the PropertyChanged event of the ViewModel.
        /// Used here to detect when the ViewModel's ID property changes after loading.
        /// </summary>
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.ID))
            {
                Debug.WriteLine($"ProductDetailPage: ViewModel.ID changed to: {ViewModel.ID}"); // Added logging
                // Although the XAML binding ProductId="{Binding ID}" should handle this,
                // this log confirms when the ID is actually updated in the ViewModel.
            }
            // You could add checks for other properties changing if needed for debugging
        }


        /// <summary>
        /// Handles the OnNavigatedTo event to load product data when the page is navigated to.
        /// </summary>
        /// <param name="e">The navigation event arguments.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Debug.WriteLine($"ProductDetailPage: OnNavigatedTo called. Parameter type: {e.Parameter?.GetType().Name}, Parameter value: {e.Parameter}"); // Added logging

            // Check if the navigation parameter is an integer (the product ID)
            if (e.Parameter is int productId)
            {
                Debug.WriteLine($"ProductDetailPage: Navigation parameter is Product ID: {productId}. Calling LoadProductAsync."); // Added logging
                // Load the product data using the ViewModel
                // Use _ = ViewModel.LoadProductAsync(...) to avoid awaiting in OnNavigatedTo
                _ = ViewModel.LoadProductAsync(productId);
            }
            else
            {
                Debug.WriteLine($"ProductDetailPage: Navigation parameter is NOT an integer Product ID. Parameter: {e.Parameter}"); // Added logging
                // You might want to handle cases where the parameter is not an int or is missing
                // For example, navigate back or show an error message.
            }
        }

        /// <summary>
        /// Handles the Click event for the "View" button on a related product item.
        /// Navigates to the detail page for the tapped related product within the current window.
        /// </summary>
        /// <param name="sender">The source of the event (the clicked Button).</param>
        /// <param name="e">The event arguments.</param>
        private void SeeRelatedProductButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("ProductDetailPage: SeeRelatedProductButton_Click called."); // Added logging
            // Get the clicked Button
            if (sender is Button clickedButton)
            {
                // Get the product ID from the Tag property
                if (clickedButton.Tag is int relatedProductId)
                {
                    Debug.WriteLine($"ProductDetailPage: Related Product Button clicked. Navigating to Product ID: {relatedProductId}"); // Added logging
                    // Initialize dependencies for the ProductService for the new page.
                    // In a real application, you would typically use a Dependency Injection container here.
                    string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    var connectionFactory = new DbConnectionFactory(connectionString);
                    var dbService = new DbService(connectionFactory);
                    var productRepository = new ProductRepository(dbService);
                    var productService = new ProductService(productRepository);

                    // Create a new instance of the ProductDetailPage for the related product
                    // Pass the *current* hosting window to the new page's constructor
                    var relatedProductPage = new ProductDetailPage(this.hostingWindow);

                    // Load the data for the related product
                    // Use _ = relatedProductPage.ViewModel.LoadProductAsync(...) to avoid awaiting in a void method
                    _ = relatedProductPage.ViewModel.LoadProductAsync(relatedProductId);

                    // Set the content of the *stored hosting window* to the new ProductDetailPage
                    this.hostingWindow.Content = relatedProductPage;

                    // Optional: Update the window title
                    this.hostingWindow.Title = $"Product Details (ID: {relatedProductId})";
                }
                else
                {
                    Debug.WriteLine($"ProductDetailPage: Related Product Button clicked, but Tag is not an int Product ID. Tag value: {clickedButton.Tag}"); // Added logging
                }
            }
            else
            {
                Debug.WriteLine($"ProductDetailPage: SeeRelatedProductButton_Click called, but sender is not a Button. Sender type: {sender?.GetType().Name}"); // Added logging
            }
        }
    }
}
