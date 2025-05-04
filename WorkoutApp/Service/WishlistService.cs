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
    using WorkoutApp.Utils.Filters;

    /// <summary>
    /// Provides services for managing the wishlist, including adding, removing, and retrieving wishlist items.
    /// </summary>
    public class WishlistService : IService<WishlistItem>
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
        public async Task<IEnumerable<WishlistItem>> GetAllAsync()
        {
            try
            {
                return await this.wishlistRepository.GetAllAsync();
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
        public async Task<WishlistItem> GetByIdAsync(int id)
        {
            try
            {
                return await this.wishlistRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve wishlist item with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Removes a specific wishlist item.
        /// </summary>
        /// <param name="wishlistItemID">The ID of the wishlist item to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DeleteAsync(int wishlistItemID)
        {
            try
            {
                return await this.wishlistRepository.DeleteAsync(wishlistItemID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove wishlist item with ID: {wishlistItemID}.", ex);
            }
        }

        /// <summary>
        /// Adds a product to the wishlist.
        /// </summary>
        /// <param name="wishlistItem">The wishlist item to add, including product details.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<WishlistItem> CreateAsync(WishlistItem wishlistItem)
        {
            try
            {
                return await this.wishlistRepository.CreateAsync(wishlistItem);
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

        /// <summary>
        /// This method is not implemented as the wishlist service does not support updating wishlist items directly.
        /// </summary>
        /// <param name="entity">The wishlist item to update, including updated product details and quantity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with the updated <see cref="WishlistItem"/> result.</returns>
        public Task<WishlistItem> UpdateAsync(WishlistItem entity)
        {
            return Task.FromResult(entity);
        }

        /// <summary>
        /// This method is not implemented as the wishlist service does not support filtering wishlist items directly.
        /// </summary>
        /// <param name="filter">The filter criteria to apply to the wishlist items.</param>
        /// <returns>A list of <see cref="WishlistItem"/> objects that match the filter criteria.</returns>
        public Task<IEnumerable<WishlistItem>> GetFilteredAsync(IFilter filter)
        {
            return Task.FromResult<IEnumerable<WishlistItem>>(new List<WishlistItem>());
        }
    }
}
