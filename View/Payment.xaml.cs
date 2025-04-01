using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WorkoutApp.View
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Payment : Window
    {
        public Payment()
        {
            this.InitializeComponent();
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SummaryName.Text = FirstNameTextBox.Text + LastNameTextBox.Text;
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SummaryName.Text = FirstNameTextBox.Text + LastNameTextBox.Text;
        }

        private void PhoneNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.Match(PhoneNumberTextBox.Text, @"^07[0-9]{8}$").Success)
                SummaryPhone.Text = PhoneNumberTextBox.Text;
            else
                SummaryPhone.Text = "Invalid Phone Number";
        }

        private void Address_TextChanged(object sender, RoutedEventArgs e)
        {
            SummaryAddress.Text = AddressTextBox.Text + "; " +
                CityTextBox.Text + "; " + (RegionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentMethodCard.IsChecked == true)
                SummaryPayment.Text = PaymentMethodCard.Content.ToString();
            else
                SummaryPayment.Text = PaymentMethodCash.Content.ToString();
        }

        private void SendOrderButtonClick(object sender, RoutedEventArgs e)
        {
            Window shoppingCart = new ShoppingCart();
            shoppingCart.Activate();
            this.Close();
        }
    }
}