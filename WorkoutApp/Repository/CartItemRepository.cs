using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;

namespace WorkoutApp.Repository
{
    public class CartItemRepository
    {
        private string connectionString = @"Data Source=DESKTOP-OR684EE;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private SqlConnection connection;

        public CartItemRepository()
        {
            this.connection = new SqlConnection(connectionString);
        }

        private int GetActiveCartId()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();

            SqlCommand getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(ID), 0) FROM Cart", connection);
            int newId = (int)getMaxIdCommand.ExecuteScalar();

            connection.Close();
            
            return newId;
        }

        public List<CartItem> GetAll()
        {
            int cartId = GetActiveCartId();

            connection.Open();
            List<CartItem> cartItems = new List<CartItem>();

            SqlCommand selectCommand = new SqlCommand("SELECT * FROM CartItem Where IsActive = 1 and CartId = " + cartId.ToString(), connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            while (reader.Read())
            {
                CartItem cartItem = new CartItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                };
                cartItems.Add(cartItem);
            }

            reader.Close();
            connection.Close();

            return cartItems;
        }

        public void ResetCart()
        {
            List<CartItem> cartItems = GetAll();

            foreach(CartItem cartItem in cartItems)
            {
                DeleteById(cartItem.Id);
            }

            int newId = GetActiveCartId();
            
            connection.Open();

            SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO Cart (Id, CustomerID, CreatedAt, IsActive) VALUES (@Id, 1, GETDATE(), 1)",
                connection
            );
            insertCommand.Parameters.AddWithValue("@Id", newId + 1);
            insertCommand.ExecuteNonQuery();

            SqlCommand updateCommand = new SqlCommand(
                "UPDATE Cart SET IsActive = 0 WHERE Id = " + newId.ToString(),
                connection
            );
            updateCommand.ExecuteNonQuery();
            connection.Close();

        }

        public void AddCartItem(int productId, int quantity)
        {
            int cartId = GetActiveCartId();
            connection.Open();

            SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO CartItem (Id, CartId, ProductId, Quantity, IsActive) VALUES (@Id, @CartId, @ProductId, @Quantity, @IsActive)",
                connection
            );

            SqlCommand getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(ID), 0) + 1 FROM CartItem", connection);
            int newId = (int)getMaxIdCommand.ExecuteScalar();

            insertCommand.Parameters.AddWithValue("@Id", newId);
            insertCommand.Parameters.AddWithValue("@CartId", cartId);
            insertCommand.Parameters.AddWithValue("@ProductId", productId);
            insertCommand.Parameters.AddWithValue("@Quantity", quantity);
            insertCommand.Parameters.AddWithValue("@IsActive", true);

            insertCommand.ExecuteNonQuery();

            connection.Close();
        }

        public void UpdateById(long id, long newQuantity)
        {
            connection.Open();

            SqlCommand updateCommand = new SqlCommand(
                "UPDATE CartItem SET Quantity = @Quantity WHERE Id = @Id",
                connection
            );

            updateCommand.Parameters.AddWithValue("@Id", id);
            updateCommand.Parameters.AddWithValue("@Quantity", newQuantity);

            updateCommand.ExecuteNonQuery();

            connection.Close();
        }

        public void DeleteById(long id)
        {
            connection.Open();

            SqlCommand updateCommand = new SqlCommand(
                "UPDATE CartItem SET IsActive = 0 WHERE ID = @Id",
                connection
            );

            updateCommand.Parameters.AddWithValue("@Id", id);

            updateCommand.ExecuteNonQuery();

            connection.Close();
        }

        public CartItem GetItemById(long id)
        {            
            int cartId = GetActiveCartId();

            connection.Open();


            SqlCommand selectCommand = new SqlCommand("SELECT * FROM CartItem Where IsActive = 1 and CartId = " + cartId.ToString() + " and ID = " + id.ToString(), connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            reader.Read();
            CartItem cartItem = new CartItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                };

            reader.Close();
            connection.Close();

            return cartItem;
        }
    }
}