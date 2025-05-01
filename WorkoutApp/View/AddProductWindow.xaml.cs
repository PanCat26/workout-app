using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddProductWindow : Window
    {
        public AddProductWindow()
        {
            this.InitializeComponent();
        }
        /*
                public event Action<IProduct> ProductAdded;
                IProduct newProduct { get; set; }
        */
        private void ProductTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedType = (ProductTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            ClothesPanel.Visibility = Visibility.Collapsed;
            FoodPanel.Visibility = Visibility.Collapsed;

            if (selectedType == "Clothes")
            {
                ClothesPanel.Visibility = Visibility.Visible;
            }
            else if (selectedType == "Food")
            {
                FoodPanel.Visibility = Visibility.Visible;
            }
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {/*
            string selectedType = (ProductTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selectedType == "Clothes")
            {
                newProduct = new ClothesProduct
                (
                    id: 0, // you can auto-generate or set later
                    name: NameTextBox.Text,
                    price: double.TryParse(PriceTextBox.Text, out double price) ? price : 0,
                    stock: 15,
                    categoryId: 1,
                    color: ColorsTextBox.Text,
                    size: SizesTextBox.Text,
                    description: DescriptionTextBox.Text,
                    fileUrl: ImageTextBox.Text,
                    isActive: true
                );
            }
            else if (selectedType == "Food")
            {

                newProduct = new FoodProduct
                (
                    id: 0,
                    name: NameTextBox.Text,
                    price: double.TryParse(PriceTextBox.Text, out double price) ? price : 0,
                    stock: 15,
                    categoryId: 2,
                    size: WeightsTextBox.Text,
                    description: DescriptionTextBox.Text,
                    fileUrl: ImageTextBox.Text,
                    isActive: true
                );
            }
            else if (selectedType == "Accessories")
            {
                newProduct = new AccessoryProduct
                (
                    id: 0,
                    name: NameTextBox.Text,
                    price: double.TryParse(PriceTextBox.Text, out double price) ? price : 0,
                    stock: 15,
                    categoryId: 3,
                    description: DescriptionTextBox.Text,
                    fileUrl: ImageTextBox.Text,
                    isActive: true
                );
            }

            ProductRepository productRepository = new ProductRepository();
            productRepository.AddProduct(newProduct);

            //ProductAdded?.Invoke(newProduct);
            MainWindow main = new MainWindow();
            this.Close();
            main.Activate();*/
        }

    }
}
