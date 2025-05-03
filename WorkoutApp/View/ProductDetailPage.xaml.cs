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

    /// <summary>
    /// Code-behind for the ProductDetailPage.xaml.
    /// </summary>
    public sealed partial class ProductDetailPage : Page // Inherit from Page
    {
        // Public ViewModel property for XAML binding
        public ProductViewModel ViewModel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailPage"/> class.
        /// </summary>
        public ProductDetailPage()
        {
            this.InitializeComponent();

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
            // CORRECTED: Instantiate ViewModel using the constructor that takes only the service.
            ViewModel = new ProductViewModel(productService);

            // Set the DataContext of the page to the ViewModel
            this.DataContext = ViewModel;

            // You can add other initialization logic here if needed,
            // similar to how you set the RemoveButtonText in DrinkDetailPage.
            // For example, logic based on user roles or product status.
        }

        /// <summary>
        /// Handles the OnNavigatedTo event to load product data when the page is navigated to.
        /// </summary>
        /// <param name="e">The navigation event arguments.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if the navigation parameter is an integer (the product ID)
            if (e.Parameter is int productId)
            {
                // Load the product data using the ViewModel
                // Use _ = ViewModel.LoadProductAsync(...) to avoid awaiting in OnNavigatedTo
                _ = ViewModel.LoadProductAsync(productId);
            }
        }
    }
}
