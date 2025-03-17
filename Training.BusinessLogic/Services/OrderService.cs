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
    }

    public class OrderService(IUnitOfWork unitOfWork) : IOrderService
    {

        public async Task<bool> PurchaseCart(PurchaseCartDto purchaseCartDto)
        {
            var cartRepo = unitOfWork.GetRepository<Cart>();
            var cartItemRepo = unitOfWork.GetRepository<CartItem>();
            var orderRepo = unitOfWork.GetRepository<Order>();
            var orderDetailRepo = unitOfWork.GetRepository<OrderDetail>();
            var stockRepo = unitOfWork.GetRepository<Stock>();
            var productRepo = unitOfWork.GetRepository<Product>();

            var cart = await cartRepo.Single(c => c.Id == purchaseCartDto.CartId && c.UserId == purchaseCartDto.UserId && !c.IsPurchased);
            if (cart == null)
            {
                return false;
            }

            var cartItems = (await cartItemRepo.QueryCondition(i => i.CartId == purchaseCartDto.CartId)).ToArray();
            if (cartItems.Length == 0)
            {
                return false;
            }
            var productIds = cartItems.Select(i => i.ProductId).ToArray();

            var products = (from pr in await productRepo.QueryAll()
                            join st in await stockRepo.QueryAll()
                            on pr.Id equals st.ProductId
                            where productIds.Contains(pr.Id)
                            select new
                            {
                                ProductId = pr.Id,
                                st.Quantity,
                                pr.UnitPrice
                            }).ToArray();

            var isValidQuantity = cartItems.All(orderItem =>
                                 products.Any(item => item.ProductId == orderItem.ProductId && item.Quantity >= orderItem.Quantity));
            if (!isValidQuantity)
            {
                return false;
            }

            var random = new Random();
            var order = new Order()
            {
                Id = random.NextInt64(1, long.MaxValue),
                CustomerId = purchaseCartDto.UserId
            };

            var orderDetails = new List<OrderDetail>();
            if (products.Length > 0)
            {
                foreach (var item in products)
                {
                    var quantity = cartItems.FirstOrDefault(i => i.ProductId == item.ProductId)?.Quantity ?? 1;

                    orderDetails.Add(new OrderDetail()
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        UnitPrice = item.UnitPrice,
                        Quantity = quantity,
                    });
                }

                order.TotalAmount = orderDetails.Sum(i => (decimal)i.UnitPrice * i.Quantity);
            }

            var stocks = (await stockRepo.QueryCondition(i => productIds.Contains(i.ProductId))).ToArray();

            var isSuccess = false;
            var strategy = unitOfWork.DbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await unitOfWork.DbContext.Database.BeginTransactionAsync();
                try
                {
                    await orderRepo.Add(order);
                    if (orderDetails.Count > 0) await orderDetailRepo.Add(orderDetails);

                    var stockUpdate = new List<Stock>();
                    foreach (var item in cartItems)
                    {
                        var stock = stocks.FirstOrDefault(i => i.ProductId == item.ProductId);
                        if (stock != null)
                        {
                            stock.Quantity -= item.Quantity;
                            stockUpdate.Add(stock);
                        }
                    }

                    if (stockUpdate.Count > 0)
                    {
                        await stockRepo.Update(stockUpdate);
                    }

                    cart.IsPurchased = true;
                    await cartRepo.Update(cart);
                    await cartItemRepo.Delete(cartItems);
                    await unitOfWork.SaveChanges();

                    await transaction.CommitAsync();
                    isSuccess = true;
                }
                catch (Exception)
                {
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync();
                    }

                    isSuccess = false;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            });

            return isSuccess;
        }

        public async Task<List<OrderDto>> GetOrders(long userId)
        {
            var orders = (await unitOfWork.GetRepository<Order>().QueryCondition(o => o.CustomerId == userId))
                                                                 .Select(i => new OrderDto()
                                                                 {
                                                                     Id = i.Id,
                                                                     CreatedAt = i.CreatedAt,
                                                                     TotalAmount = i.TotalAmount,
                                                                 }).ToList();

            if (orders.Count > 0)
            {
                var orderIds = orders.Select(o => o.Id).ToArray();
                var orderDetails = await (from od in await unitOfWork.GetRepository<OrderDetail>().QueryAll()
                                          join pr in await unitOfWork.GetRepository<Product>().QueryAll()
                                          on od.ProductId equals pr.Id
                                          where orderIds.Contains(od.OrderId)
                                          select new OrderDetailDTO
                                          {
                                              Id = od.Id,
                                              OrderId = od.OrderId,
                                              ProductId = od.ProductId,
                                              UnitPrice = od.UnitPrice,
                                              Quantity = od.Quantity,
                                              ProductName = pr.Name,
                                              Thumbnail = pr.Thumbnail,
                                          }).ToListAsync();

                if (orderDetails.Count > 0)
                {
                    var orderDetailsDict = orderDetails
                                          .GroupBy(od => od.OrderId)
                                          .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var order in orders)
                    {
                        if (orderDetailsDict.TryGetValue(order.Id, out var details))
                        {
                            order.OrderDetails = details;
                        }
                    }
                }
            }

            return orders;
        }
    }
}
