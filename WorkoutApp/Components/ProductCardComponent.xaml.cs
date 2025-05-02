using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.Components
{
    public sealed partial class ProductCardComponent : UserControl
    {
        /*
        private Window parent { get; set; }
        private IProduct product { get; set; }
        */
        public ProductCardComponent()
        {
            this.InitializeComponent();
        }
        /*
        public ProductCardComponent(IProduct product, Window parent)
        {
            this.InitializeComponent();
            this.product = product;
            this.parent = parent;
            this.DataContext = new CartItemViewModel(product);
        }
        */
        private void SeeProduct_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
