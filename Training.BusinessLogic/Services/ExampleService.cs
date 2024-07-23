using AutoMapper;
using Training.BusinessLogic.Dtos.Examples;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services
{
    public interface IExampleService
    {
        Task<ExampleDto?> Get(long id);

        Task<ExampleDto> Insert(ExampleDto exampleDto);
    }

    public class ExampleService(
        IMapper mapper,
        IUnitOfWork unitOfWork) : IExampleService
    {
        public async Task<ExampleDto?> Get(long id)
        {
            var query = await unitOfWork.GetRepository<Example>().QueryAll();
            var example = query.FirstOrDefault(x => x.Id == id);
            return mapper.Map<ExampleDto>(example);
        }

        public async Task<ExampleDto> Insert(ExampleDto exampleDto)
        {
            var repo = unitOfWork.GetRepository<Example>();

            var exampleEntity = mapper.Map<Example>(exampleDto);

            await repo.Add(exampleEntity);
            await unitOfWork.SaveChanges();

            return mapper.Map<ExampleDto>(exampleEntity);
        }
    }
}
