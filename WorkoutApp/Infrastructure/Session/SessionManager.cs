// <copyright file="SessionManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Infrastructure.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Manages user sessions.
    /// </summary>
    public class SessionManager
    {
        /// <summary>
        /// Gets or sets the ID of the currently logged-in user.
        /// </summary>
        public int? CurrentUserId { get; set; }

        /// <summary>
        /// Gets a value indicating whether a user is currently logged in.
        /// </summary>
        public bool IsUserLoggedIn => this.CurrentUserId.HasValue;

        /// <summary>
        /// Clears the current session, logging the user out.
        /// </summary>
        public void ClearSession()
        {
            this.CurrentUserId = null;
        }
    }
}
