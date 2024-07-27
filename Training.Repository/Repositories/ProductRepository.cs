using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.DataAccess.DbContexts;
using Training.DataAccess.Entities;

namespace Training.Repository.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
    }
    public class ProductRepository(MyDbContext context)
        : BaseRepository<Product>(context), IProductRepository
    {
    }
}
