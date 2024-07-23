using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Products")]
    public class Product : IBaseEntity, IIsDeletedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public float UnitPrice { get; set; }
        public long CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public bool IsDeleted { get; set; }

        public long UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }


        public User CreatedByUser { get; set; }
        public Category Category { get; set; }
   
    }

}
