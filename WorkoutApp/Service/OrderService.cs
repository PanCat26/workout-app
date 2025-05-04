// <copyright file="OrderService.cs" company="PlaceholderCompany">
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
    /// Service class for handling Order-related operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </remarks>
    /// <param name="orderRepository">The order repository.</param>
    /// <param name="cartRepository">The cart item repository.</param>
    public class OrderService(IRepository<Order> orderRepository, IRepository<CartItem> cartRepository) : IService<Order>
    {
        private readonly IRepository<Order> orderRepository = orderRepository;
        private readonly IRepository<CartItem> cartRepository = cartRepository;

        /// <inheritdoc/>
        public async Task<Order> CreateAsync(Order entity)
        {
            return await this.orderRepository.CreateAsync(entity);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            return await this.orderRepository.DeleteAsync(id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await this.orderRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<Order> GetByIdAsync(int id)
        {
            return (await this.orderRepository.GetByIdAsync(id)) !;
        }

        /// <inheritdoc/>
        public Task<Order> UpdateAsync(Order entity)
        {
            return this.orderRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// Creates an order from the current items in the cart.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateOrderFromCartAsync()
        {
            IEnumerable<CartItem> cartItems = await this.cartRepository.GetAllAsync();
            List<OrderItem> orderItems = [];
            foreach (CartItem cartItem in cartItems)
            {
                await this.cartRepository.DeleteAsync((int)cartItem.ID!);
                orderItems.Add(new OrderItem(cartItem.Product, 1));
            }

            Order newOrder = new (null, orderItems, DateTime.Now);
            await this.orderRepository.CreateAsync(newOrder);
        }
    }
}
