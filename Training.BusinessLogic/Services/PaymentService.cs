using Microsoft.Extensions.Configuration;
using Stripe;
using Training.BusinessLogic.Dtos.Customers;
using Training.BusinessLogic.Dtos.Payment;
using Training.Common.Constants;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services
{
    public interface IPaymentService
    {
        Task<CartDto?> CreateOrUpdatePaymentIntent(long userId);

        Task<DeliveryMethodResDto[]> GetAllDeliveryMethod();
    }

    public class PaymentService(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ICartService cartService) : IPaymentService
    {
        private readonly string _publishableKey = configuration.GetSection(ConfigKeys.StripeSettings.PublishableKey).Get<string>()!;
        private readonly string _serectKey = configuration.GetSection(ConfigKeys.StripeSettings.SerectKey).Get<string>()!;

        public async Task<CartDto?> CreateOrUpdatePaymentIntent(long userId)
        {
            StripeConfiguration.ApiKey = _serectKey;

            var cart = await cartService.GetCart(userId);
            if(cart == null)
            {
                return null;
            }

            var shippingPrice = 0m;

            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod>().FindById(cart.DeliveryMethodId.Value);
                if (deliveryMethod == null)
                {
                    return null;
                }

                shippingPrice = deliveryMethod.Price;
            }

            var service = new PaymentIntentService();
            PaymentIntent? intent = null;

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)cart.CartItems.Sum(i => i.Quantity * i.Price) + (long)shippingPrice,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                intent = await service.CreateAsync(options);

                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;

                var cartEntity = await unitOfWork.GetRepository<Cart>().FindById(cart.Id);
                if (cartEntity != null)
                {
                    cartEntity.PaymentIntentId = cart.PaymentIntentId;
                    cartEntity.ClientSecret = cart.ClientSecret;
                    await unitOfWork.GetRepository<Cart>().Update(cartEntity);
                    await unitOfWork.SaveChanges();
                }
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)cart.CartItems.Sum(i => i.Quantity * i.Price) + (long)shippingPrice,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                intent = await service.UpdateAsync(cart.PaymentIntentId, options);
            }

            return cart;
        }

        public async Task<DeliveryMethodResDto[]> GetAllDeliveryMethod()
        {
            var deliveryMethods = (await unitOfWork.GetRepository<DeliveryMethod>().QueryAll())
                                                   .Select(i => new DeliveryMethodResDto()
                                                   {
                                                       Id = i.Id,
                                                       ShortName = i.ShortName,
                                                       DeliveryTime = i.DeliveryTime,
                                                       Description = i.Description,
                                                       Price = i.Price,
                                                   }).ToArray();

            return deliveryMethods;
        }
    }
}
