using Microsoft.EntityFrameworkCore;
using Training.DataAccess.Entities;

namespace Training.Repository.Repositories
{
    public interface IExampleRepository : IBaseRepository<Example>
    {
    }

    public class ExampleRepository(DbContext context)
        : BaseRepository<Example>(context), IExampleRepository
    {
    }
}
