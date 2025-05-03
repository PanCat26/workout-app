using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using WorkoutApp.Data.Database;
using WorkoutApp.Repository;
using WorkoutApp.Service;
using WorkoutApp.ViewModel;

namespace WorkoutApp.View
{
    public sealed partial class AddProductFlyout : UserControl
    {
        private readonly AddProductViewModel viewModel;

        public AddProductFlyout(ProductService productService, CategoryService categoryService)
        {
            this.InitializeComponent();

            this.viewModel = new AddProductViewModel(productService);
            this.DataContext = viewModel;

            _ = LoadCategories(categoryService);
        }

        private async Task LoadCategories(CategoryService categoryService)
        {
            await viewModel.LoadCategoriesAsync(categoryService);
        }

        private async void OnAddProductClick(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AddProductViewModel vm)
            {
                bool result = await viewModel.AddProductAsync();

                var dialog = new ContentDialog
                {
                    Title = result ? "Success" : "Error",
                    Content = result
                        ? "Product was added successfully."
                        : viewModel.ValidationMessage ?? "There was an error adding the product. Check debug logs for details.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            }

            System.Diagnostics.Debug.WriteLine("[AddProductFlyout] Add Product button clicked.");
        }

    }
}
