// <copyright file="WishlistService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;

    /// <summary>
    /// Provides services for managing the wishlist, including adding, removing, and retrieving wishlist items.
    /// </summary>
    public class WishlistService
    {
        private readonly IRepository<WishlistItem> wishlistRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistService"/> class.
        /// </summary>
        /// <param name="wishlistRepository">The repository for managing wishlist items.</param>
        public WishlistService(IRepository<WishlistItem> wishlistRepository)
        {
            this.wishlistRepository = wishlistRepository;
        }

        /// <summary>
        /// Retrieves all items in the wishlist.
        /// </summary>
        /// <returns>A list of <see cref="WishlistItem"/> objects representing the items in the wishlist.</returns>
        public async Task<List<WishlistItem>> GetWishlistItems()
        {
            try
            {
                return (List<WishlistItem>)await this.wishlistRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve wishlist items.", ex);
            }
        }

        /// <summary>
        /// Retrieves a specific wishlist item by its ID.
        /// </summary>
        /// <param name="id">The ID of the wishlist item to retrieve.</param>
        /// <returns>The <see cref="WishlistItem"/> with the specified ID.</returns>
        public async Task<WishlistItem> GetWishlistItemById(int id)
        {
            try
            {
                WishlistItem item = await this.wishlistRepository.GetByIdAsync(id)
                                        ?? throw new KeyNotFoundException($"Wishlist item with ID {id} not found.");
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve wishlist item with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Removes a specific wishlist item.
        /// </summary>
        /// <param name="wishlistItem">The wishlist item to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveWishlistItem(WishlistItem wishlistItem)
        {
            try
            {
                await this.wishlistRepository.DeleteAsync(wishlistItem.ID ?? throw new InvalidOperationException("WishlistItem ID cannot be null."));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove wishlist item with ID: {wishlistItem.ID}.", ex);
            }
        }

        /// <summary>
        /// Adds a product to the wishlist.
        /// </summary>
        /// <param name="wishlistItem">The wishlist item to add, including product details.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AddToWishlist(WishlistItem wishlistItem)
        {
            try
            {
                await this.wishlistRepository.CreateAsync(wishlistItem);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add product {wishlistItem.Product.ID} to wishlist.", ex);
            }
        }

        /// <summary>
        /// Clears all items from the wishlist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ResetWishlist()
        {
            try
            {
                IEnumerable<WishlistItem> wishlistItems = await this.wishlistRepository.GetAllAsync();
                foreach (WishlistItem item in wishlistItems)
                {
                    await this.wishlistRepository.DeleteAsync(item.ID ?? throw new InvalidOperationException("WishlistItem ID cannot be null."));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to reset wishlist.", ex);
            }
        }
    }
}
