using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;
using WorkoutApp.Repository;

namespace WorkoutApp.Service
{
    public class WishlistService
    {
        WishlistItemRepository wishlistItemRepository;
        ProductRepository productRepository;

        public WishlistService(WishlistItemRepository wishlistItemRepository, ProductRepository productRepository)
        {
            this.wishlistItemRepository = wishlistItemRepository;
            this.productRepository = productRepository;
        }

        public List<WishlistItem> GetWishlistItems()
        {
            //return wishlistItemRepository.GetAll();
            return [];
        }


        public void addToWishlist(int productId)
        {
            /*List<WishlistItem> wishlistitems = this.wishlistItemRepository.GetAll();
            if (wishlistitems.Find(item => item.ProductID == productId) == null)
                this.wishlistItemRepository.AddWishlistItem(productId);
            else
                throw new Exception("This product is already in the wishlist!");*/

        }


        public void RemoveWishlistItem(WishlistItem wishlistItem)
        {
            //wishlistItemRepository.DeleteById(wishlistItem.ID);
        }
    }
}


