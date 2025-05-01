/*using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;
using WorkoutApp.Repository;

namespace WorkoutApp.Service
{
    public class OrderService
    {
        private string connectionString = @"Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private SqlConnection connection;

        public OrderService()
        {
            this.connection = new SqlConnection(connectionString);
        }

        public void SendOrder(double TotalAmount)
        {
            *//*connection.Open();

            SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO [Order] (ID, CustomerId, OrderDate, TotalAmount, IsActive) VALUES (@ID, @CustomerId, GETDATE(), @TotalAmount, @IsActive)",
                connection
            );

            SqlCommand getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(ID), 0) + 1 FROM [Order]", connection);
            int newId = (int)getMaxIdCommand.ExecuteScalar();

            insertCommand.Parameters.AddWithValue("@Id", newId);
            insertCommand.Parameters.AddWithValue("@CustomerId", 1);
            insertCommand.Parameters.AddWithValue("@TotalAmount", TotalAmount);
            insertCommand.Parameters.AddWithValue("@IsActive", true);

            insertCommand.ExecuteNonQuery();

            connection.Close();

            CartItemRepository cartItemRepository = new CartItemRepository();
            ProductRepository productRepository = new ProductRepository();

            var cartItems = cartItemRepository.GetAll();

            foreach (var cartItem in cartItems)
            {
                addOrderDetail(newId, (int) cartItem.ProductId, (int) cartItem.Quantity, cartItem.GetProduct(productRepository).Price);
            }

            cartItemRepository.ResetCart();*//*
        }

        private void addOrderDetail(int OrderID, int ProductID, int Quantity, double Price)
        {
            connection.Open();

            SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO OrderDetail (Id, OrderID, ProductId, Quantity, Price, IsActive) VALUES (@Id, @OrderID, @ProductId, @Quantity, @Price, @IsActive)",
                connection
            );

            SqlCommand getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(ID), 0) + 1 FROM OrderDetail", connection);
            int newId = (int)getMaxIdCommand.ExecuteScalar();

            insertCommand.Parameters.AddWithValue("@Id", newId);
            insertCommand.Parameters.AddWithValue("@OrderID", OrderID);
            insertCommand.Parameters.AddWithValue("@ProductId", ProductID);
            insertCommand.Parameters.AddWithValue("@Quantity", Quantity);
            insertCommand.Parameters.AddWithValue("@Price", Price);
            insertCommand.Parameters.AddWithValue("@IsActive", true);

            insertCommand.ExecuteNonQuery();

            connection.Close();
        }
    }
}
*/