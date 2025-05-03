using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WorkoutApp.Data.Database;
using WorkoutApp.Repository;
using WorkoutApp.Service;

namespace WorkoutApp.View
{
    public sealed partial class AddProductButton : UserControl
    {
        public AddProductButton()
        {
            this.InitializeComponent();
        }

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
                Content = new AddProductFlyout(productService, categoryService)
            };

            flyout.ShowAt(AddButton);
        }
    }
}
