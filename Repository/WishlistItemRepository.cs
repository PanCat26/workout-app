using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml.Automation;
using WorkoutApp.Models;

namespace WorkoutApp.Repository
{
    public class WishlistItemRepository
    {
        private string connectionString = @"Data Source=FLORIN;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        private SqlConnection connection;

        public WishlistItemRepository()
        {
            this.connection = new SqlConnection(connectionString);
        }

        private int GetActiveCustomerId()
        {
            return 1;
        }

        public List<WishlistItem> GetAll()
        {
            connection.Open();
            List<WishlistItem> wishListItems = new List<WishlistItem>();


            int customerId = GetActiveCustomerId();

            SqlCommand selectCommand = new SqlCommand("SELECT * FROM Wishlist Where IsActive = 1 and CustomerID = " + customerId.ToString(), connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            while (reader.Read())
            {
                WishlistItem item = new WishlistItem
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                };
                wishListItems.Add(item);
            }

            reader.Close();
            connection.Close();

            return wishListItems;
        }

        public void AddWishlistItem(int productId)
        {
            connection.Open();

            SqlCommand insertCommand = new SqlCommand(
                "INSERT INTO Wishlist (Id,  ProductID, CustomerID, IsActive) VALUES (@Id, @ProductID, @CustomerID, @IsActive)",
                connection
            );
            SqlCommand getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(ID), 0) + 1 FROM Wishlist", connection);
            int newId = (int)getMaxIdCommand.ExecuteScalar();

            insertCommand.Parameters.AddWithValue("@ID",newId);
            insertCommand.Parameters.AddWithValue("@ProductID", productId);
            insertCommand.Parameters.AddWithValue("@CustomerID", this.GetActiveCustomerId());
            insertCommand.Parameters.AddWithValue("@IsActive", 1);

            insertCommand.ExecuteNonQuery();

            connection.Close();
        }



        

        public void DeleteById(int id)
        {
            connection.Open();

            SqlCommand updateCommand = new SqlCommand(
                "UPDATE Wishlist SET IsActive = 0 WHERE ID = @Id",
                connection
            );

            updateCommand.Parameters.AddWithValue("@Id", id);

            updateCommand.ExecuteNonQuery();

            connection.Close();
        }

    }
}
