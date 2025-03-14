using System.ComponentModel.DataAnnotations.Schema;

namespace Training.BusinessLogic.Dtos.Payment
{
    public class DeliveryMethodResDto
    {
        public long Id { get; set; }

        public required string ShortName { get; set; }

        public required string DeliveryTime { get; set; }

        public required string Description { get; set; }

        public decimal Price { get; set; }
    }
}
