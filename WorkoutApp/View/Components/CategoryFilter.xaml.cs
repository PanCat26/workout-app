// <copyright file="CategoryFilter.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View.Components
{
    using System;
    using System.Configuration;
    using Microsoft.UI.Xaml.Controls;
    using WorkoutApp.Data.Database;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;
    using WorkoutApp.Service;

    /// <summary>
    /// Represents a filter control for selecting workout categories.
    /// </summary>
    public sealed partial class CategoryFilter : UserControl
    {
        private readonly CategoryFilterViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryFilter"/> class.
        /// </summary>
        public CategoryFilter()
        {
            this.InitializeComponent();

            var connectionFactory = new DbConnectionFactory(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var dbService = new DbService(connectionFactory);
            var categoryRepository = new CategoryRepository(dbService);
            var categoryService = new CategoryService(categoryRepository);

            this.viewModel = new CategoryFilterViewModel(categoryService);
            this.DataContext = this.viewModel;

            this.Loaded += (_, __) =>
            {
                this.DispatcherQueue.TryEnqueue(async () =>
                {
                    await this.viewModel.LoadCategoriesAsync();
                });
            };
        }

        /// <summary>
        /// Occurs when the selected category changes.
        /// </summary>
        public event EventHandler<int> CategoryChanged;

        /// <summary>
        /// Resets the category filter to its default state.
        /// </summary>
        public void ResetFilter()
        {
            this.CategoryComboBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Handles the selection change event of the category list view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is Category selectedCategory && selectedCategory.ID.HasValue)
            {
                this.CategoryChanged?.Invoke(this, selectedCategory.ID.Value);
            }
        }
    }
}
