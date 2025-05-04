using Microsoft.UI.Xaml.Controls;
using WorkoutApp.Service;
using WorkoutApp.Repository;
using WorkoutApp.Data.Database;
using System.Configuration;
using WorkoutApp.Models;
using System;

namespace WorkoutApp.View.Components
{
    public sealed partial class CategoryFilter : UserControl
    {
        private readonly CategoryFilterViewModel viewModel;

        public event EventHandler<int> CategoryChanged;

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

        private void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is Category selectedCategory && selectedCategory.ID.HasValue)
            {
                this.CategoryChanged?.Invoke(this, selectedCategory.ID.Value);
            }
        }

        public void ResetFilter()
        {
            this.CategoryComboBox.SelectedIndex = -1;
        }
    }
}
