// WorkoutApp.View.Components/DeleteProductButton.xaml.cs
// <copyright file="DeleteProductButton.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.View.Components // Your specified namespace
{
    using System.Diagnostics; // Required for Debug.WriteLine
    using Microsoft.UI.Xaml; // Required for DependencyProperty, DependencyPropertyChangedEventArgs
    using Microsoft.UI.Xaml.Controls; // Required for UserControl, Button
    using WorkoutApp.ViewModel; // Assuming ProductViewModel is here

    /// <summary>
    /// A custom button component specifically for Delete actions, receiving its ViewModel externally.
    /// </summary>
    public sealed partial class DeleteProductButton : UserControl // Inherit from UserControl
    {
        // Removed ProductId Dependency Property as we are passing the ViewModel directly
        /*
        public static readonly DependencyProperty ProductIdProperty =
            DependencyProperty.Register(
                nameof(ProductId),
                typeof(int),
                typeof(DeleteProductButton),
                new PropertyMetadata(0, OnProductIdChanged));
        */

        /// <summary>
        /// Identifies the <see cref="ViewModel"/> DependencyProperty.
        /// This property holds the instance of the ProductViewModel for this button.
        /// The ViewModel is provided by the parent View.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(ProductViewModel),
                typeof(DeleteProductButton),
                new PropertyMetadata(null, OnViewModelPropertyChanged)); // Default value null, call OnViewModelPropertyChanged when changed

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteProductButton"/> class.
        /// </summary>
        public DeleteProductButton()
        {
            Debug.WriteLine("DeleteProductButton: Constructor called."); // Added logging
            this.InitializeComponent(); // Initialize the XAML defined in DeleteProductButton.xaml

            // Removed Loaded event subscription as ViewModel initialization is external
            // Loaded += DeleteProductButton_Loaded;
        }

        // Removed ProductId property
        /*
        public int ProductId
        {
            get => (int)GetValue(ProductIdProperty);
            set => SetValue(ProductIdProperty, value);
        }
        */

        /// <summary>
        /// Gets or sets the instance of the ProductViewModel for this button.
        /// This is a Dependency Property. Setting this property will update the DataContext.
        /// </summary>
        public ProductViewModel ViewModel
        {
            get => (ProductViewModel)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }

        // Removed OnProductIdChanged as ProductId DP is removed
        /*
        private static void OnProductIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             // ... logic removed ...
        }
        */

        /// <summary>
        /// Handles changes to the <see cref="ViewModel"/> DependencyProperty.
        /// Updates the DataContext of the UserControl when the ViewModel is set.
        /// </summary>
        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine($"DeleteProductButton: OnViewModelPropertyChanged called. NewValue is {(e.NewValue == null ? "null" : "not null")}."); // Added logging
            if (d is DeleteProductButton button && e.NewValue is ProductViewModel newViewModel)
            {
                Debug.WriteLine($"DeleteProductButton: ViewModel property set. Updating DataContext."); // Added logging
                button.DataContext = newViewModel;
            }
        }

        // Removed Loaded event handler as ViewModel initialization is external
        /*
        private async void DeleteProductButton_Loaded(object sender, RoutedEventArgs e)
        {
            // ... logic removed ...
        }
        */

        // The Click event handler is still handled by x:Bind in DeleteProductButton.xaml:
        // Click="{x:Bind ViewModel.ExecuteDeleteAsync}"
        // This directly calls the ExecuteDeleteAsync method on the ViewModel instance
        // that is set as the DataContext of the UserControl via the ViewModel DP.
        // No separate Click event handler is needed in the code-behind for this pattern.
    }
}
