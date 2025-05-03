using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WorkoutApp.ViewModel;
using WorkoutApp.Service;
using WorkoutApp.Repository;
using WorkoutApp.Data.Database;
using System.Configuration;



namespace WorkoutApp.View
{
    public sealed partial class CategoryFilter : UserControl
    {
        private readonly CategoryFilterViewModel viewModel;

        public CategoryFilter()
        {
            this.InitializeComponent();

            System.Diagnostics.Debug.WriteLine("[CategoryFilter] Constructor called");

            var connectionFactory = new DbConnectionFactory(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var dbService = new DbService(connectionFactory); 
            var categoryRepository = new CategoryRepository(dbService); 
            var categoryService = new CategoryService(categoryRepository);

            this.viewModel = new CategoryFilterViewModel(categoryService);
            this.DataContext = viewModel;

            this.Loaded += (_, __) =>
            {
                System.Diagnostics.Debug.WriteLine("[CategoryFilter] Loaded event triggered");

                this.DispatcherQueue.TryEnqueue(async () =>
                {
                    await viewModel.LoadCategoriesAsync();
                });
            };
        }

    }
}
