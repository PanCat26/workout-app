/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Repository;
using WorkoutApp.Models;

namespace WorkoutApp.Service
{
    public class CartService
    {
        CartItemRepository cartItemRepository;
        ProductRepository productRepository;

        public CartService(CartItemRepository cartItemRepository, ProductRepository productRepository) {
            this.cartItemRepository = cartItemRepository;
            this.productRepository = productRepository;
        }

        public List<CartItem> GetCartItems()
        {
            //return cartItemRepository.GetAll();
            return [];
        }


        public CartItem GetCartItemById(int id)
        {
            //return cartItemRepository.GetItemById(id);
            return new CartItem();
        }

        public void IncreaseQuantity(CartItem cartItem)
        {
            //cartItemRepository.UpdateById(cartItem.Id, cartItem.Quantity + 1);
        }

        public void DecreaseQuantity(CartItem cartItem)
        {
            *//*if(cartItem.Quantity > 0) 
                cartItemRepository.UpdateById(cartItem.Id, cartItem.Quantity - 1);
            else 
                cartItemRepository.DeleteById(cartItem.Id);*//*
        }

        public void RemoveCartItem(CartItem cartItem)
        {
            //cartItemRepository.DeleteById(cartItem.Id);
        }

        public void AddToCart(int productId, int quantity)
        {
            //cartItemRepository.AddCartItem(productId, quantity);
        }
    }
}
*/