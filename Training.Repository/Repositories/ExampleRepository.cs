using Microsoft.EntityFrameworkCore;
using Training.DataAccess.DbContexts;
using Training.DataAccess.Entities;

namespace Training.Repository.Repositories
{
    public interface IExampleRepository : IBaseRepository<Example>
    {
    }

    public class ExampleRepository(MyDbContext context)
        : BaseRepository<Example>(context), IExampleRepository
    {
    }
}
