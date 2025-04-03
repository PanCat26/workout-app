using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using System.Drawing;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Windows.ApplicationModel.Chat;
using WorkoutApp.Models;

namespace WorkoutApp.Repository
{
    public class ProductRepository
    {
       private string loginString= @"Data Source=FLORIN;Initial Catalog=ShopDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

        private SqlConnection connection;
        private List<IProduct> products;

        public ProductRepository()
        {
            this.connection = new SqlConnection(loginString);
            this.products = new List<IProduct>();
            this.LoadData();
        }

        public List<IProduct> GetProducts()
        {
            return this.products;
        }
        public void LoadData()
        {
            products.Clear();
            connection.Open();

            SqlCommand selectCommand = new SqlCommand("Select * from Product where IsActive = 1", connection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            while (reader.Read()) {
                int category = reader.GetInt32(reader.GetOrdinal("CategoryID"));
                IProduct product = null;

                if (category == 1)
                {
                    product = new ClothesProduct
                    (
                        id: reader.GetInt32(reader.GetOrdinal("ID")),
                        name: reader.GetString(reader.GetOrdinal("Name")),
                        price: reader.GetDouble(reader.GetOrdinal("Price")),
                        stock: reader.GetInt32(reader.GetOrdinal("Stock")),
                        categoryId: category,
                        color: reader.GetString(reader.GetOrdinal("Atributes")),
                        size: reader.GetString(reader.GetOrdinal("Size")),
                        description: reader.GetString(reader.GetOrdinal("Description")),
                        fileUrl: reader.GetString(reader.GetOrdinal("FileUrl")),
                        isActive: true
                    );

                }

                else if (category == 2)
                {
                    product = new FoodProduct
                     (
                        id: reader.GetInt32(reader.GetOrdinal("ID")),
                        name: reader.GetString(reader.GetOrdinal("Name")),
                        price: reader.GetDouble(reader.GetOrdinal("Price")),
                        stock: reader.GetInt32(reader.GetOrdinal("Stock")),
                        categoryId: category,
                        size: reader.GetString(reader.GetOrdinal("Size")),
                        description: reader.GetString(reader.GetOrdinal("Description")),
                        fileUrl: reader.GetString(reader.GetOrdinal("FileUrl")),
                        isActive: true
                    );

                }
                else
                {
                    product = new AccessoryProduct
                    (
                        id: reader.GetInt32(reader.GetOrdinal("ID")),
                        name: reader.GetString(reader.GetOrdinal("Name")),
                        price: reader.GetDouble(reader.GetOrdinal("Price")),
                        stock: reader.GetInt32(reader.GetOrdinal("Stock")),
                        categoryId: category,
                        description: reader.GetString(reader.GetOrdinal("Description")),
                        fileUrl: reader.GetString(reader.GetOrdinal("FileUrl")),
                        isActive: true
                    );
                }

                products.Add(product);

                }

            reader.Close();
            connection.Close();

        }

        public void DeleteById(int productId)
        {
            connection.Open();
            
            SqlCommand deleteProductCommand = new SqlCommand(
                "UPDATE Product SET IsActive = 0 WHERE ID = @ID ", connection
            );
            deleteProductCommand.Parameters.AddWithValue("@ID", productId);
            deleteProductCommand.ExecuteNonQuery();

            connection.Close();
            LoadData();
        }

        public void UpdateProduct(int productId, string name, double price, int stock, int categoryId, string description, string fileUrl, string color, string size )
        {
            connection.Open();

            SqlCommand updateCommand = new SqlCommand(
                "UPDATE Product SET Name = @Name, Price = @Price, Description = @Description, " +
                " Stock = @Stock , FileUrl = @FileUrl,  CategoryID = @CategoryID, Atributes = @Atributes, Size = @Size  WHERE ID = @ID;", connection
            );
            updateCommand.Parameters.AddWithValue("@ID", productId);
            updateCommand.Parameters.AddWithValue("@Name", name);
            updateCommand.Parameters.AddWithValue("@Price", price);
            updateCommand.Parameters.AddWithValue("@FileUrl", fileUrl);
            updateCommand.Parameters.AddWithValue("@Description", description);
            updateCommand.Parameters.AddWithValue("@Size", size);
            updateCommand.Parameters.AddWithValue("@Stock", stock);
            updateCommand.Parameters.AddWithValue("@CategoryID", categoryId);
            updateCommand.Parameters.AddWithValue("@Atributes", color);


            updateCommand.ExecuteNonQuery();

            connection.Close();
            LoadData();
        }

        public List<IProduct> GetAll()
        {
            return this.products;
        }
        public IProduct GetById(int id)
        {
            return  products.Find(p => p.ID == id);

        }

    }
}

