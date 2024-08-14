using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Training.BusinessLogic.Dtos.Customers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services
{
    public interface IOrderService
    {
        Task<bool> PurchaseCart(PurchaseCartDto purchaseCartDto);
        Task<List<OrderDto>> GetOrders(long userId);
        //Task<OrderDto> GetOrderDetails(long orderId);

    }

    public class OrderService(IMapper mapper,
        IUnitOfWork unitOfWork) : IOrderService
    {

        public async Task<bool> PurchaseCart(PurchaseCartDto purchaseCartDto)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                var cartRepo = unitOfWork.GetRepository<Cart>();
                var cartItemRepo = unitOfWork.GetRepository<CartItem>();
                var orderRepo = unitOfWork.GetRepository<Order>();
                var orderDetailRepo = unitOfWork.GetRepository<OrderDetail>();
                var stockRepo = unitOfWork.GetRepository<Stock>();

                var cart = await cartRepo.Single(c => c.Id == purchaseCartDto.CartId && c.UserId == purchaseCartDto.UserId && !c.IsPurchased,
                                                            include: c => c.Include(c => c.CartItems).ThenInclude(ci=> ci.Product));

                if (cart == null)
                {
                    return false;
                }

                var order = new Order
                {
                    CustomerId = purchaseCartDto.UserId,
                    CreatedAt = DateTimeOffset.Now,
                    OrderDetails = new List<OrderDetail>()
                };
                await orderRepo.Add(order);
                await unitOfWork.SaveChanges();

                foreach(var item in cart.CartItems)
                {
                    var stock = await stockRepo.Single(s => s.ProductId == item.ProductId);
                    if (stock == null || stock.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock quantity for product {item.ProductId}.");
                    }

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        UnitPrice = item.Product.UnitPrice,
                        Quantity = item.Quantity,

                    };
                    order.OrderDetails.Add(orderDetail);
                    await orderDetailRepo.Add(orderDetail);


                    // Update stock quantity
                    stock.Quantity -= item.Quantity;
                    await stockRepo.Update(stock);

                    // Soft delete cart item
                    item.IsDeleted = true;
                    await cartItemRepo.Update(item);

                }

                // Mark cart as purchased
                cart.IsPurchased = true;
                cart.IsDeleted = true;
                await cartRepo.Update(cart);

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

        public async Task<List<OrderDto>> GetOrders(long userId)
        {
            var orderRepo = unitOfWork.GetRepository<Order>();

            var query = await orderRepo.QueryCondition(o => o.CustomerId == userId);

            var orders = await query.Include(o => o.OrderDetails)
                                    .ThenInclude(od => od.Product).ToListAsync(); 


            return mapper.Map<List<OrderDto>>(orders);  

        }

        //public async Task<OrderDto> GetOrderDetails(long orderId)
        //{
        //    var orderRepo = unitOfWork.GetRepository<Order>();
        //    var order = await orderRepo.Single(o => o.Id == orderId && o.CustomerId == userId && !o.IsDeleted,
        //                                       include: o => o.Include(ord => ord.OrderDetails)
        //                                                      .ThenInclude(od => od.Product));
        //    return order;
        //}

    }
}
