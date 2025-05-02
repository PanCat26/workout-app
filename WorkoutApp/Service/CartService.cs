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

    /// <summary>
    /// Provides services for managing the shopping cart, including adding, removing, and updating cart items.
    /// </summary>
    public class CartService
    {
        private readonly CartRepository cartRepository;
        private readonly ProductRepository productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartService"/> class.
        /// </summary>
        /// <param name="cartItemRepository">The repository for managing cart items.</param>
        /// <param name="productRepository">The repository for managing products.</param>
        public CartService(CartRepository cartItemRepository, ProductRepository productRepository)
        {
            this.cartRepository = cartItemRepository;
            this.productRepository = productRepository;
        }

        /// <summary>
        /// Retrieves all items in the shopping cart.
        /// </summary>
        /// <returns>A list of <see cref="CartItem"/> objects representing the items in the cart.</returns>
        public async Task<List<CartItem>> GetCartItems()
        {
            try
            {
                return (List<CartItem>)await this.cartRepository.GetAllAsync();
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
        public async Task<CartItem> GetCartItemById(int id)
        {
            try
            {
                CartItem item = await this.cartRepository.GetByIdAsync(id)
                                 ?? throw new KeyNotFoundException($"Cart item with ID {id} not found.");
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve cart item with ID {id}.", ex);
            }
        }

        /// <summary>
        /// Increases the quantity of a specific cart item by 1.
        /// </summary>
        /// <param name="cartItem">The cart item to update.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="CartItem"/> result.</returns>
        public async Task IncreaseQuantity(CartItem cartItem)
        {
            try
            {
                cartItem.Quantity += 1;
                await this.cartRepository.UpdateAsync(cartItem);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to increase cart item quantity.", ex);
            }
        }

        /// <summary>
        /// Decreases the quantity of a specific cart item by 1. If the quantity becomes 0, the item is removed from the cart.
        /// </summary>
        /// <param name="cartItem">The cart item to update.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="CartItem"/> result.</returns>
        public async Task DecreaseQuantity(CartItem cartItem)
        {
            try
            {
                cartItem.Quantity -= 1;
                if (cartItem.Quantity > 0)
                {
                    await this.cartRepository.UpdateAsync(cartItem);
                }
                else
                {
                    await this.cartRepository.DeleteAsync(cartItem.ProductID);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to decrease cart item quantity.", ex);
            }
        }

        /// <summary>
        /// Removes a specific cart item from the cart.
        /// </summary>
        /// <param name="cartItem">The cart item to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="CartItem"/> result.</returns>
        public async Task RemoveCartItem(CartItem cartItem)
        {
            try
            {
                await this.cartRepository.DeleteAsync(cartItem.ProductID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove cart item with ProductID {cartItem.ProductID}.", ex);
            }
        }

        /// <summary>
        /// Adds a product to the cart with the specified quantity.
        /// </summary>
        /// <param name="productId">The ID of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a <see cref="CartItem"/> result.</returns>
        public async Task AddToCart(int productId, int quantity)
        {
            try
            {
                await this.cartRepository.CreateAsync(productId, quantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add product {productId} to cart.", ex);
            }
        }
    }
}
