// <copyright file="CartService.cs" company="PlaceholderCompany">
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
    /// Provides services for managing the shopping cart, including adding, removing, and updating cart items.
    /// </summary>
    public class CartService : IService<CartItem>
    {
        private readonly IRepository<CartItem> cartRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartService"/> class.
        /// </summary>
        /// <param name="cartItemRepository">The repository for managing cart items.</param>
        public CartService(IRepository<CartItem> cartItemRepository)
        {
            this.cartRepository = cartItemRepository;
        }

        /// <summary>
        /// Retrieves all items in the shopping cart.
        /// </summary>
        /// <returns>A list of <see cref="CartItem"/> objects representing the items in the cart.</returns>
        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            try
            {
                return await this.cartRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve cart items.", ex);
            }
        }

        /// <summary>
        /// Retrieves a specific cart item by its ID.
        /// </summary>
        /// <param name="id">The ID of the cart item to retrieve.</param>
        /// <returns>The <see cref="CartItem"/> with the specified ID.</returns>
        public async Task<CartItem> GetByIdAsync(int id)
        {
            try
            {
                return await this.cartRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve cart item with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Removes a specific cart item from the cart.
        /// </summary>
        /// <param name="cartItemID">The ID of the cart item to be remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="CartItem"/> result.</returns>
        public async Task<bool> DeleteAsync(int cartItemID)
        {
            try
            {
                return await this.cartRepository.DeleteAsync(cartItemID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove cart item with ID: {cartItemID}.", ex);
            }
        }

        /// <summary>
        /// Adds a product to the cart with the specified quantity.
        /// </summary>
        /// <param name="cartItem">The cart item to add to the cart, including product details and quantity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="CartItem"/> result.</returns>
        public async Task<CartItem> CreateAsync(CartItem cartItem)
        {
            try
            {
                return await this.cartRepository.CreateAsync(cartItem);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add product {cartItem.Product.ID} to cart.", ex);
            }
        }

        /// <summary>
        /// Resets the shopping cart by clearing all items.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ResetCart()
        {
            try
            {
                IEnumerable<CartItem> cartItems = await this.cartRepository.GetAllAsync();
                foreach (CartItem item in cartItems)
                {
                    await this.cartRepository.DeleteAsync(item.ID ?? throw new InvalidOperationException("CartItem ID cannot be null."));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to reset cart.", ex);
            }
        }

        /// <summary>
        /// This method is not implemented as the cart service does not support updating cart items directly.
        /// </summary>
        /// <param name="entity">The cart item to update, including updated product details and quantity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with the updated <see cref="CartItem"/> result.</returns>
        public Task<CartItem> UpdateAsync(CartItem entity)
        {
            return Task.FromResult(entity);
        }

        /// <summary>
        /// This method is not implemented as the cart service does not support filtering cart items directly.
        /// </summary>
        /// <param name="filter">The filter criteria to apply to the cart items.</param>
        /// <returns>A list of <see cref="CartItem"/> objects that match the filter criteria.</returns>
        public Task<IEnumerable<CartItem>> GetFilteredAsync(IFilter filter)
        {
            return Task.FromResult<IEnumerable<CartItem>>(new List<CartItem>());
        }
    }
}
