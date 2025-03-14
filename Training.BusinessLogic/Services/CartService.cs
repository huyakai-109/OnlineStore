using AutoMapper;
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
            var cart = await unitOfWork.GetRepository<Cart>().Single(c => c.UserId == userId && !c.IsPurchased);
            if (cart == null) return null;

            var cartDto = new CartDto();

            var products = (from ca in await unitOfWork.GetRepository<CartItem>().QueryAll()
                           join pr in await unitOfWork.GetRepository<Product>().QueryAll()
                           on ca.ProductId equals pr.Id
                           where ca.CartId == cart.Id
                           orderby ca.UpdatedAt
                           select new CartItemDto()
                           {
                               Id = ca.Id,
                               ProductId = ca.ProductId,
                               ProductName = pr.Name,
                               Quantity = ca.Quantity,
                               Price = pr.UnitPrice,
                               Thumbnail = pr.Thumbnail,
                           }).ToList();

            cartDto.Id = cart.Id;
            cartDto.CartItems = products;
            cartDto.ClientSecret = cart.ClientSecret;
            cartDto.PaymentIntentId = cart.PaymentIntentId;

            return cartDto;
        }

        public async Task<bool> EditQuantity(EditCartQuantityDto editCartQuantityDto)
        {
            var cartRepo = unitOfWork.GetRepository<Cart>();
            var cartItemRepo = unitOfWork.GetRepository<CartItem>();
            var stockRepo = unitOfWork.GetRepository<Stock>();

            // get user's cart
            var cart = await cartRepo.Single(c => c.UserId == editCartQuantityDto.UserId && !c.IsPurchased);
            if (cart == null)
            {
                return false;
            }

            // check product in cart
            var cartItem = cart.CartItems.Single(ci => ci.ProductId == editCartQuantityDto.ProductId && ci.CartId == cart.Id);
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

            return true;
        }

        public async Task<bool> RemoveProduct(RemoveProductFCartDto removeProductFCartDto)
        {
            var cartRepo = unitOfWork.GetRepository<Cart>();
            var cartItemRepo = unitOfWork.GetRepository<CartItem>();

            //Get user's cart
            var cart = await cartRepo.Single(c => c.UserId == removeProductFCartDto.UserId && !c.IsPurchased);
            if (cart == null)
            {
                return false;
            }

            // Find the cart item to remove
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == removeProductFCartDto.ProductId && cart.Id == ci.CartId);
            if (cartItem == null)
            {
                return false;
            }

            await cartItemRepo.Delete(cartItem);
            await unitOfWork.SaveChanges();

            return true;
        }
    }
}
