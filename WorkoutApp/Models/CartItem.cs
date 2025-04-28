// <copyright file="CartItem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace WorkoutApp.Models
{
    using WorkoutApp.Repository;

    /// <summary>
    /// Represents an item inside a shopping cart.
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartItem"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the cart item.</param>
        /// <param name="cartId">The unique identifier for the cart.</param>
        /// <param name="productId">The unique identifier for the product.</param>
        /// <param name="quantity">The quantity of the item.</param>
        public CartItem(long id, long cartId, long productId, long quantity)
        {
            this.Id = id;
            this.CartId = cartId;
            this.ProductId = productId;
            this.Quantity = quantity;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the cart item.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the cart to which this item belongs.
        /// </summary>
        public long CartId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the product associated with this cart item.
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product in the cart item.
        /// </summary>
        public long Quantity { get; set; }

        /// <summary>
        /// Retrieves the <see cref="IProduct"/> associated with this cart item using the provided product repository.
        /// </summary>
        /// <param name="repo">The product repository used to fetch the product details.</param>
        /// <returns>The product associated with this cart item.</returns>
        public IProduct GetProduct(ProductRepository repo)
        {
            repo.LoadData();
            return repo.GetById((int)this.ProductId);
        }
    }
}
