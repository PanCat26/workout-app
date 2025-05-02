using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutApp.Models;
using WorkoutApp.Repository;

namespace WorkoutApp.Service
{
    public class OrderService : IService<Order>
    {
        IRepository<Order> orderRepository;
        //IRepository<CartItem> cartRepository;

        public OrderService(IRepository<Order> orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<Order> CreateAsync(Order entity)
        {
            return await this.orderRepository.CreateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await this.orderRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await this.orderRepository.GetAllAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await this.orderRepository.GetByIdAsync(id);
        }

        public void CreateOrderFromCart()
        {
            //CreateOrderFromCart(): ia tot din cartrepo, il goleste, si apeleaza add din orderrepo, aici se face order si se ia timestamp
        }

        public void SendOrder(double TotalAmount)
        {
            /*connection.Open();

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
                addOrderDetail(newId, (int)cartItem.ProductId, (int)cartItem.Quantity, cartItem.GetProduct(productRepository).Price);
            }

            cartItemRepository.ResetCart();*/
        }

        public Task<Order> UpdateAsync(Order entity)
        {
            return this.orderRepository.UpdateAsync(entity);
        }

        private void addOrderDetail(int OrderID, int ProductID, int Quantity, double Price)
        {
            /*//call creasteAsync(OrderDetail)
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

            connection.Close();*/
        }
    }
}
