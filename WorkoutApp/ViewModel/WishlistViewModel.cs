using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using WorkoutApp.Data.Database;
using WorkoutApp.Infrastructure.Session;
using WorkoutApp.Models;
using WorkoutApp.Repository;
using WorkoutApp.Service;

namespace WorkoutApp.ViewModel
{
    public class WishlistViewModel
    {
        private readonly IService<WishlistItem> wishlistService;

        public WishlistViewModel()
        {
            string? connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string 'DefaultConnection' is not configured or is null.");
            }
            DbConnectionFactory dbConnectionFactory = new DbConnectionFactory(connectionString);
            DbService dbService = new DbService(dbConnectionFactory);
            SessionManager sessionManager = new SessionManager();
            IRepository<WishlistItem> wishlistRepository = new WishlistItemRepository(dbService, sessionManager);
            this.wishlistService = new WishlistService(wishlistRepository);
        }

        public async Task<IEnumerable<WishlistItem>> GetAllProductsFromWishlistAsync()
        {
            IEnumerable<WishlistItem> wishlistItems = await this.wishlistService.GetAllAsync();
            return wishlistItems;
        }

        public async Task<WishlistItem> AddProductToWishlist(Product product)
        {
            return await this.wishlistService.CreateAsync(new WishlistItem(null, product, 1));
        }

        public async Task<bool> RemoveProductFromWishlist(int wishlistItemID)
        {
            return await this.wishlistService.DeleteAsync(wishlistItemID);
        }
    }
}
