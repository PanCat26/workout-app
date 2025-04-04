using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using WorkoutApp.Models;
using WorkoutApp.Service;
using WorkoutApp.ViewModel;
using WorkoutApp.View.ProductTab;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.Components
{
    public sealed partial class ProductCardComponent : UserControl
    {
        private Window parent { get; set; }
        private IProduct product { get; set; }

        public ProductCardComponent()
        {
            this.InitializeComponent();
        }

        public ProductCardComponent(IProduct product, Window parent)
        {
            this.InitializeComponent();
            this.product = product;
            this.parent = parent;
            this.DataContext = new CartItemViewModel(product);
        }

        private void SeeProduct_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();    
        }
    }
}
