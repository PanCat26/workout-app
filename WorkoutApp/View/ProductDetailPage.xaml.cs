// <copyright file="ProductDetailPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View // Using the 'View' namespace as in your provided code
{
    using System.Configuration;
    using Microsoft.UI.Xaml; // Required for RoutedEventArgs - USED FOR BUTTON CLICKS
    using Microsoft.UI.Xaml.Controls; // For WinUI Page
    using Microsoft.UI.Xaml.Navigation; // For NavigationEventArgs
    using WorkoutApp.Data.Database; // Assuming DbConnectionFactory and DbService are here
    using WorkoutApp.Repository; // Assuming ProductRepository and IRepository are here
    using WorkoutApp.Service; // Assuming ProductService and IService are here
    using WorkoutApp.ViewModel; // Corrected: Using the singular 'ViewModel' namespace for ProductViewModel

    // Removed using Microsoft.UI.Xaml.Media; as VisualTreeHelper is no longer needed for this approach
    // Removed using WinRT; as As<> extension method is no longer needed for this approach

    /// <summary>
    /// Code-behind for the ProductDetailPage.xaml.
    /// </summary>
    public sealed partial class ProductDetailPage : Page // Inherit from Page
    {
        /// <summary>
        /// The ViewModel for the ProductDetailPage.
        /// </summary>
        public ProductViewModel ViewModel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailPage"/> class.
        /// </summary>
        public ProductDetailPage()
        {
            this.InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var connectionFactory = new DbConnectionFactory(connectionString);
            var dbService = new DbService(connectionFactory);
            var productRepository = new ProductRepository(dbService);
            var productService = new ProductService(productRepository);

            // Initialize the ViewModel with the necessary service
            this.ViewModel = new ProductViewModel(productService);

            // Set the DataContext of the page to the ViewModel
            this.DataContext = this.ViewModel;

            // You can add other initialization logic here if needed,
            // similar to how you set the RemoveButtonText in DrinkDetailPage.
            // For example, logic based on user roles or product status.
        }

        /// <summary>
        /// Handles the OnNavigatedTo event to load product data when the page is navigated to.
        /// </summary>
        /// <param name="e">The navigation event arguments.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if the navigation parameter is an integer (the product ID)
            if (e.Parameter is int productId)
            {
                // Load the product data using the ViewModel
                // Use _ = ViewModel.LoadProductAsync(...) to avoid awaiting in OnNavigatedTo
                await this.ViewModel.LoadProductAsync(productId);
            }
        }

        /// <summary>
        /// Handles the Click event for the "View" button on a related product item.
        /// Navigates to the detail page for the tapped related product within the current window.
        /// </summary>
        /// <param name="sender">The source of the event (the clicked Button).</param>
        /// <param name="e">The event arguments.</param>
        private async void SeeRelatedProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the clicked Button
            if (sender is Button clickedButton)
            {
                // Get the product ID from the Tag property
                if (clickedButton.Tag is int relatedProductId)
                {
                    await this.ViewModel.LoadProductAsync(relatedProductId);
                }
            }
        }
    }
}
