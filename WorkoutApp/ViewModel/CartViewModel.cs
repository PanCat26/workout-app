using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Threading.Tasks;
using WorkoutApp.Data.Database;
using WorkoutApp.Infrastructure.Session;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;

namespace WorkoutApp.ViewModel
{
    public class CartViewModel : INotifyPropertyChanged
    {
        private readonly IService<CartItem> cartService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public decimal TotalPrice { get; private set; } = 0;

        public CartViewModel()
        {
            string? connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string 'DefaultConnection' is not configured or is null.");
            }

            DbConnectionFactory dbConnectionFactory = new DbConnectionFactory(connectionString);
            DbService dbService = new DbService(dbConnectionFactory);
            SessionManager sessionManager = new SessionManager();
            IRepository<CartItem> cartRepository = new CartRepository(dbService, sessionManager);
            this.cartService = new CartService(cartRepository);
        }

        public async Task<IEnumerable<CartItem>> GetAllProductsFromCartAsync()
        {
            IEnumerable<CartItem> cartItems = await this.cartService.GetAllAsync();
            this.ComputeTotalPrice(cartItems);
            return cartItems;
        }

        public async Task<CartItem> AddProductToCart(Product product)
        {
            return await this.cartService.CreateAsync(new CartItem(null, product, 1));
        }


        private async void ComputeTotalPrice(IEnumerable<CartItem> cartItems)
        {
            this.TotalPrice = 0;
            foreach (CartItem item in cartItems)
            {
                this.TotalPrice += item.Product.Price;
            }
        }


    }
}
