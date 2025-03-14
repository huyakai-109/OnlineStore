using AutoMapper;
using Training.Api.Models.Responses.Payments;
using Training.BusinessLogic.Dtos.Payment;

namespace Training.Api.Mappers
{
    public class PaymentMapper : Profile
    {
        public PaymentMapper()
        {
            CreateMap<DeliveryMethodResDto, DeliveryMethodRes>();
        }
    }
}
