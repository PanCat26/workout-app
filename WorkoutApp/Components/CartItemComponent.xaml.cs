// <copyright file="CartItemComponent.xaml.cs" company="YourCompanyName">
// Copyright (c) YourCompanyName. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace WorkoutApp.Components
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;

    // using WorkoutApp.Service;
    using WorkoutApp.ViewModel;

    /// <summary>
    /// Represents a component for displaying and managing a cart item in the WorkoutApp.
    /// </summary>
    public sealed partial class CartItemComponent : UserControl
    {
        private StackPanel parent { get; set; }

        private Func<int> callBack { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartItemComponent"/> class.
        /// </summary>
        public CartItemComponent()
        {
            this.InitializeComponent();
            this.parent = new StackPanel(); // Initialize with a default value
            this.callBack = () => 0; // Initialize with a default value
        }

        /// <summary>
        /// Handles the click event for decreasing the quantity of the cart item.
        /// </summary>
        private void deacreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the click event for increasing the quantity of the cart item.
        /// </summary>
        private void increaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the click event for removing the product from the cart.
        /// </summary>
        private void removeProductButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
