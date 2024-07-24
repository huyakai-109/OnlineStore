using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Common.EnumTypes;
using Training.DataAccess.IEntities;

namespace Training.DataAccess.Entities
{
    [Table("Users")]
    public class User: IIsDeletedEntity, IBaseEntity
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CivilianId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserRole Role { get; set; }
        public bool IsDeleted { get; set ;}

        public long CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long UpdatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
