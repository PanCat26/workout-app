// <copyright file="IMockRepository.cs" company="WorkoutApp">
// Copyright (c) WorkoutApp. All rights reserved.
// </copyright>

namespace WorkoutApp.Repository
{
    using System.Collections.Generic;
    using WorkoutApp.Models;

    /// <summary>
    /// Defines the mock repository interface for managing <see cref="MockModel"/> entities.
    /// </summary>
    public interface IMockRepository
    {
        /// <summary>
        /// Retrieves all mock models.
        /// </summary>
        /// <returns>A collection of <see cref="MockModel"/>.</returns>
        IEnumerable<MockModel> GetAll();

        /// <summary>
        /// Adds a new mock model to the repository.
        /// </summary>
        /// <param name="model">The mock model to add.</param>
        void Add(MockModel model);

        /// <summary>
        /// Saves all changes to the repository.
        /// </summary>
        void Save();
    }
}
