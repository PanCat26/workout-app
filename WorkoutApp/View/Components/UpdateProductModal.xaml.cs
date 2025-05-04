// WorkoutApp.View.Components/UpdateProductModal.xaml.cs
// <copyright file="UpdateProductModal.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View.Components // Your specified namespace
{
    using System.Diagnostics; // Required for Debug.WriteLine
    using Microsoft.UI.Xaml; // Required for DependencyPropertyChangedEventArgs, DependencyObject
    using Microsoft.UI.Xaml.Controls; // Required for UserControl

    /// <summary>
    /// Interaction logic for UpdateProductModal.xaml
    /// This UserControl serves as the content for the update product dialog.
    /// Its DataContext is expected to be set to a ProductViewModel.
    /// </summary>
    public sealed partial class UpdateProductModal : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProductModal"/> class.
        /// </summary>
        public UpdateProductModal()
        {
            Debug.WriteLine("UpdateProductModal: Constructor called."); // Added logging
            this.InitializeComponent();

            // The DataContext will be set externally by the ContentDialog or the parent page.

            // Subscribe to the DataContextChanged event to know when the ViewModel is set
            this.DataContextChanged += this.UpdateProductModal_DataContextChanged;

            // Subscribe to the Loaded event to know when the control is added to the visual tree
            this.Loaded += this.UpdateProductModal_Loaded;
        }

        /// <summary>
        /// Handles the DataContextChanged event.
        /// </summary>
        private void UpdateProductModal_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Debug.WriteLine($"UpdateProductModal: DataContextChanged event fired. New DataContext is {(args.NewValue == null ? "null" : args.NewValue.GetType().Name)}."); // Added logging
            if (args.NewValue is ViewModel.ProductViewModel vm)
            {
                Debug.WriteLine($"UpdateProductModal: DataContext set to ProductViewModel. Current Name: {vm.Name}, Price: {vm.Price}, Stock: {vm.Stock}"); // Log some properties
            }
        }

        /// <summary>
        /// Handles the Loaded event.
        /// </summary>
        private void UpdateProductModal_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"UpdateProductModal: Loaded event fired. Current DataContext is {(this.DataContext == null ? "null" : this.DataContext.GetType().Name)}."); // Added logging
            if (this.DataContext is ViewModel.ProductViewModel vm)
            {
                Debug.WriteLine($"UpdateProductModal: DataContext is ProductViewModel on Loaded. Current Name: {vm.Name}, Price: {vm.Price}, Stock: {vm.Stock}"); // Log some properties
            }
        }
    }
}
