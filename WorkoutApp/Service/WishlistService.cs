// <copyright file="WishlistService.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
// </copyright>
namespace WorkoutApp.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;

    /// <summary>
    /// Service class for managing wishlist operations.
    /// </summary>
    public class WishlistService : IService<WishlistItem>
    {
        private readonly IRepository<WishlistItem> wishlistItemRepository;
        private readonly IRepository<Product> productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistService"/> class.
        /// </summary>
        /// <param name="wishlistItemRepository">The wishlist item repository.</param>
        /// <param name="productRepository">The product repository.</param>
        public WishlistService(IRepository<WishlistItem> wishlistItemRepository, IRepository<Product> productRepository)
        {
            this.wishlistItemRepository = wishlistItemRepository ?? throw new ArgumentNullException(nameof(wishlistItemRepository));
            this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        /// <summary>
        /// Gets all wishlist items for the current customer asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with a collection of wishlist items.</returns>
        public async Task<IEnumerable<WishlistItem>> GetAllAsync()
        {
            return await wishlistItemRepository.GetAllAsync();
        }

        /// <summary>
        /// Gets a wishlist item by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the wishlist item.</param>
        /// <returns>A task representing the asynchronous operation with the wishlist item.</returns>
        public async Task<WishlistItem> GetByIdAsync(int id)
        {
            return await wishlistItemRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Creates a new wishlist item asynchronously.
        /// </summary>
        /// <param name="entity">The wishlist item to create.</param>
        /// <returns>A task representing the asynchronous operation with the created wishlist item.</returns>
        public async Task<WishlistItem> CreateAsync(WishlistItem entity)
        {
            return await wishlistItemRepository.CreateAsync(entity);
        }

        /// <summary>
        /// Updates an existing wishlist item asynchronously.
        /// </summary>
        /// <param name="entity">The wishlist item to update.</param>
        /// <returns>A task representing the asynchronous operation with the updated wishlist item.</returns>
        public async Task<WishlistItem> UpdateAsync(WishlistItem entity)
        {
            return await wishlistItemRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// Deletes a wishlist item by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the wishlist item to delete.</param>
        /// <returns>A task representing the asynchronous operation with a boolean indicating success.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            return await wishlistItemRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Adds a product to the wishlist asynchronously.
        /// </summary>
        /// <param name="productId">The ID of the product to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the product is already in the wishlist.</exception>
        public async Task AddToWishlistAsync(int productId)
        {
            var wishlistitems = await this.wishlistItemRepository.GetAllAsync();
            foreach (var item in wishlistitems)
            {
                if (item.ProductID == productId)
                {
                    throw new Exception("This product is already in the wishlist!");
                }
            }

            var wishlistItem = new WishlistItem
            {
                ProductID = productId
            };

            await this.wishlistItemRepository.CreateAsync(wishlistItem);
        }

        /// <summary>
        /// Removes an item from the wishlist asynchronously.
        /// </summary>
        /// <param name="wishlistItem">The wishlist item to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveWishlistItemAsync(WishlistItem wishlistItem)
        {
            await wishlistItemRepository.DeleteAsync(wishlistItem.ID);
        }
    }
}