// <copyright file="CartService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WorkoutApp.Models;
    using WorkoutApp.Repository;

    /// <summary>
    /// Provides services for managing cart items and interacting with the cart repository.
    /// </summary>
    public class CartService
    {
        private readonly CartItemRepository cartItemRepository;
        private readonly ProductRepository productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartService"/> class.
        /// </summary>
        /// <param name="cartItemRepository">The repository for managing cart items.</param>
        /// <param name="productRepository">The repository for managing products.</param>
        public CartService(CartItemRepository cartItemRepository, ProductRepository productRepository)
        {
            this.cartItemRepository = cartItemRepository;
            this.productRepository = productRepository;
        }

        /// <summary>
        /// Retrieves all cart items asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation, containing a list of <see cref="CartItem"/>.</returns>
        public async Task<List<CartItem>> GetCartItemAsync()
        {
            return (List<CartItem>)await this.cartItemRepository.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a cart item by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the cart item to retrieve.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation, containing the <see cref="CartItem"/>.</returns>
        public async Task<CartItem> GetCartItemByIdAsync(int id)
        {
            return await this.cartItemRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Increases the quantity of a specified cart item asynchronously.
        /// </summary>
        /// <param name="cartItem">The cart item whose quantity will be increased.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task IncreaseQuantityAsync(CartItem cartItem)
        {
            cartItem.Quantity += 1;
            await this.cartItemRepository.UpdateAsync(cartItem);
        }

        /// <summary>
        /// Decreases the quantity of a cart item asynchronously. Deletes it from the cart if the quantity becomes zero.
        /// </summary>
        /// <param name="cartItem">The cart item whose quantity will be decreased.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DecreaseQuantityAsync(CartItem cartItem)
        {
            if (cartItem.Quantity > 0)
            {
                cartItem.Quantity -= 1;
                await this.cartItemRepository.UpdateAsync(cartItem);
            }
            else
            {
                await this.cartItemRepository.DeleteAsync(cartItem.Id);
            }
        }

        /// <summary>
        /// Removes a cart item asynchronously.
        /// </summary>
        /// <param name="cartItem">The cart item to be removed from the cart.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task RemoveCartItemAsync(CartItem cartItem)
        {
            await this.cartItemRepository.DeleteAsync(cartItem.Id);
        }

        /// <summary>
        /// Adds a product to the cart asynchronously.
        /// </summary>
        /// <param name="productId">The ID of the product to be added to the cart.</param>
        /// <param name="quantity">The quantity of the product to be added.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task AddToCartAsync(int productId, int quantity)
        {
            await this.cartItemRepository.CreateAsync(productId, quantity);
        }
    }
}
