// <copyright file="AddProductButton.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Repository;
    using WorkoutApp.Service;

    /// <summary>
    /// Represents a button control that allows adding a product.
    /// </summary>
    public sealed partial class AddProductButton : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddProductButton"/> class.
        /// </summary>
        public AddProductButton()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the click event for the Add Product button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var connectionFactory = new DbConnectionFactory(connectionString);
            var dbService = new DbService(connectionFactory);
            var categoryRepo = new CategoryRepository(dbService);
            var productRepo = new ProductRepository(dbService);
            var productService = new ProductService(productRepo);
            var categoryService = new CategoryService(categoryRepo);

            var flyout = new Flyout
            {
                Content = new AddProductFlyout(productService, categoryService),
            };

            flyout.ShowAt(this.AddButton);
        }
    }
}
