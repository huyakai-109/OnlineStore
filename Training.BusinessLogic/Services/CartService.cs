using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Dtos.Customers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services
{
    public interface ICartService
    {
        Task<CartDto?> GetCart(long userId);
        Task<bool> EditQuantity(EditCartQuantityDto editCartQuantityDto);
        Task<bool> RemoveProduct(RemoveProductFCartDto removeProductFCartDto);
    }
    public class CartService(IUnitOfWork unitOfWork, IMapper mapper) : ICartService
    {
        public async Task<CartDto?> GetCart(long userId)
        {
           /* var cart = await unitOfWork.GetRepository<Cart>().Single(c => c.UserId == userId && !c.IsPurchased);
            if (cart == null) return null;

            var cartItems = await unitOfWork.GetRepository<CartItem>().QueryAllWithIncludes(ci => ci.CartId == cart.Id, disableTracking: true, ci => ci.Product);
            if (cartItems == null) return null;

            var cartDto = mapper.Map<CartDto>(cart);
            cartDto.CartItems = mapper.Map<List<CartItemDto>>(await cartItems.ToListAsync());

            return cartDto;*/

            var cartRepo = unitOfWork.GetRepository<Cart>();
            var cart = await cartRepo.Single(c => c.UserId == userId && !c.IsPurchased,
                                                           include: c => c.Include(c => c.CartItems).ThenInclude(ci => ci.Product));
            if (cart == null) return null;

            return mapper.Map<CartDto>(cart);
        }

        public async Task<bool> EditQuantity(EditCartQuantityDto editCartQuantityDto)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                var cartRepo = unitOfWork.GetRepository<Cart>();
                var cartItemRepo = unitOfWork.GetRepository<CartItem>();
                var stockRepo = unitOfWork.GetRepository<Stock>();

                // get user's cart
                var cart = await cartRepo.Single(c => c.UserId == editCartQuantityDto.UserId && !c.IsPurchased, include: c => c.Include(c => c.CartItems));
                if (cart == null)
                {
                    return false;
                }

                // check product in cart
                var cartItem =  cart.CartItems.FirstOrDefault(ci => ci.ProductId == editCartQuantityDto.ProductId);
                if (cartItem == null)
                {
                    return false;
                }

                // check quantity in stock
                var stock = await stockRepo.Single(s => s.ProductId == editCartQuantityDto.ProductId);
                if (stock == null || stock.Quantity < editCartQuantityDto.NewQuantity)
                {
                    throw new InvalidOperationException("Insufficient stock quantity.");
                }

                // update quantity in cart
                cartItem.Quantity = editCartQuantityDto.NewQuantity;
                await cartItemRepo.Update(cartItem);

                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> RemoveProduct(RemoveProductFCartDto removeProductFCartDto)
        {

            using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                var cartRepo = unitOfWork.GetRepository<Cart>();
                var cartItemRepo = unitOfWork.GetRepository<CartItem>();

                //Get user's cart
                var cart = await cartRepo.Single(c => c.UserId == removeProductFCartDto.UserId && !c.IsPurchased, include: c => c.Include(c => c.CartItems));
                if (cart == null)
                {
                    return false;
                }
                // Find the cart item to remove
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == removeProductFCartDto.ProductId);
                if (cartItem == null)
                {
                    return false;
                }

                cart.CartItems.Remove(cartItem);
                await cartItemRepo.Delete(cartItem);

                await unitOfWork.SaveChanges();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
