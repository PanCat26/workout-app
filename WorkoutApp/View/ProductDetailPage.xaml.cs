// <copyright file="ProductDetailPage.xaml.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
// </copyright>

namespace WorkoutApp.View // Using the 'View' namespace as in your provided code
{
    using System.Configuration;
    using Microsoft.UI.Xaml.Controls; // For WinUI Page, ContentDialog
    using Microsoft.UI.Xaml.Navigation; // For NavigationEventArgs
    using WorkoutApp.Data.Database; // Assuming DbConnectionFactory and DbService are here
    using WorkoutApp.Repository; // Assuming ProductRepository and IRepository are here
    using WorkoutApp.Service; // Assuming ProductService and IService are here
    using WorkoutApp.ViewModel; // Corrected: Using the singular 'ViewModel' namespace for ProductViewModel
    using Microsoft.UI.Xaml; // Required for RoutedEventArgs, FrameworkElement
    using System; // Required for System namespace
    using System.Diagnostics; // Required for Debug.WriteLine
    using System.ComponentModel; // Required for PropertyChangedEventArgs
    using Microsoft.UI.Dispatching; // Required for DispatcherQueue

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

            // Subscribe to the ViewModel's events
            ViewModel.PropertyChanged += ViewModel_PropertyChanged; // Keep for other property changes if needed
            ViewModel.RequestShowUpdateModal += ViewModel_RequestShowUpdateModal; // Subscribe to the new event
            Debug.WriteLine("ProductDetailPage: Subscribed to ViewModel.PropertyChanged and RequestShowUpdateModal."); // Added logging
        }

        /// <summary>
        /// Handles the PropertyChanged event of the ViewModel.
        /// This handler is kept in case you need to react to other property changes,
        /// but it no longer directly shows the modal based on IsUpdateModalOpen.
        /// </summary>
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"ProductDetailPage: ViewModel.PropertyChanged called for: {e.PropertyName}"); // Added logging
                                                                                                           // The logic to show the modal based on IsUpdateModalOpen is moved to ViewModel_RequestShowUpdateModal
        }

        /// <summary>
        /// Handles the RequestShowUpdateModal event from the ViewModel.
        /// This method is responsible for showing the ContentDialog.
        /// </summary>
        private void ViewModel_RequestShowUpdateModal(object? sender, EventArgs e)
        {
            Debug.WriteLine("ProductDetailPage: ViewModel_RequestShowUpdateModal event received."); // Added logging

            // Use DispatcherQueue.TryEnqueue to schedule the ShowAsync call
            // back onto the UI thread's dispatcher queue. This is necessary
            // to avoid potential re-entrancy issues.
            DispatcherQueue.TryEnqueue(async () =>
            {
                Debug.WriteLine("ProductDetailPage: DispatcherQueue Enqueue callback executing."); // Added logging inside enqueue

                // Explicitly set the DataContext of the ContentDialog's content (the UpdateProductModal)
                // This ensures the modal has the correct ViewModel instance for binding.
                // Cast Content to FrameworkElement to access DataContext
                if (UpdateProductContentDialog.Content is FrameworkElement modalContent) // Corrected: Cast to FrameworkElement
                {
                    modalContent.DataContext = ViewModel; // Corrected: Access DataContext on the casted object
                    Debug.WriteLine($"ProductDetailPage: Explicitly set DataContext of ContentDialog.Content to ViewModel."); // Added logging
                }
                else
                {
                    Debug.WriteLine("ProductDetailPage: ContentDialog.Content is null or not a FrameworkElement. Cannot set DataContext."); // Added logging
                }


                // Show the dialog
                // Use the XamlRoot from the page to show the dialog correctly
                // Ensure XamlRoot is available (page is loaded and in the visual tree)
                if (this.XamlRoot != null)
                {
                    UpdateProductContentDialog.XamlRoot = this.XamlRoot;
                    Debug.WriteLine("ProductDetailPage: Calling ShowAsync for UpdateProductContentDialog."); // Added logging before ShowAsync
                    try
                    {
                        await UpdateProductContentDialog.ShowAsync();
                        Debug.WriteLine("ProductDetailPage: UpdateProductContentDialog closed."); // Added logging after ShowAsync
                                                                                                  // When the dialog is closed (by clicking Save or Cancel),
                                                                                                  // the IsUpdateModalOpen property in the ViewModel should be set back to false
                                                                                                  // by the respective ExecuteSaveAsync or ExecuteCancelEditAsync methods.
                                                                                                  // This ensures the ViewModel state is consistent with the UI.
                    }
                    catch (Exception ex)
                    {
                        // Catch potential exceptions if ShowAsync is called while already open
                        // or other UI thread issues.
                        Debug.WriteLine($"ProductDetailPage: Error showing ContentDialog: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine("ProductDetailPage: XamlRoot is null. Cannot show ContentDialog.");
                }
            });
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
                    string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    var connectionFactory = new DbConnectionFactory(connectionString);
                    var dbService = new DbService(connectionFactory);
                    var productRepository = new ProductRepository(dbService);
                    var productService = new ProductService(productRepository);

                    // Create a new instance of the ProductDetailPage, passing the *current* hosting window to its constructor
                    // This allows the ProductDetailPage to know its hosting window for internal navigation.
                    var relatedProductPage = new ProductDetailPage(this.hostingWindow);

                    // If ProductDetailPage ViewModel has a LoadProductAsync(int id) method:
                    // This is the preferred approach for passing data after page creation.
                    // You'll need to ensure your ProductDetailPage.xaml.cs has a public ViewModel property.
                    // Use _ = ViewModel.LoadProductAsync(...) to avoid awaiting in a void method.
                    _ = relatedProductPage.ViewModel.LoadProductAsync(relatedProductId);


                    // Set the content of the new window to the ProductDetailPage
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
