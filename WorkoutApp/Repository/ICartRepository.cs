// <copyright file="ICartRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Repository
{
    using System.Threading.Tasks;
    using WorkoutApp.Models;

    /// <summary>
    /// Interface for managing cart-related operations.
    /// </summary>
    public interface ICartRepository : IRepository<CartItem>
    {
        /// <summary>
        /// Resets the cart by clearing all items.
        /// </summary>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> ResetCart();

        /// <summary>
        /// Creates a new cart item with the specified product ID and quantity asynchronously.
        /// </summary>
        /// <param name="productId">The ID of the product to add to the cart.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CreateAsync(int productId, int quantity);
    }
}
