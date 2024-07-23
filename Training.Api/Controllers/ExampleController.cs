using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Models.Requests.Examples;
using Training.Api.Models.Responses.Base;
using Training.Api.Models.Responses.Examples;
using Training.BusinessLogic.Dtos.Examples;
using Training.BusinessLogic.Services;

namespace Training.Api.Controllers
{
    [Route("api/example")]
    public class ExampleController(
        ILogger<ExampleController> logger,
        IMapper mapper,
        IExampleService exampleService) : BaseController(logger, mapper)
    {
        [HttpGet]
        [ProducesResponseType(typeof(ResultRes<ExampleNewRes?>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExample(long id)
        {
            var response = new ResultRes<ExampleNewRes?>();
            try
            {
                if (id <= 0)
                {
                    response.Error = $"Invalid Id value: {id}";
                    return BadRequest(response);
                }

                response.Result = Mapper.Map<ExampleNewRes>(await exampleService.Get(id));

                if (response.Result == null)
                {
                    response.Error = $"Unable to find entity with Id: {id}";
                    return NotFound(response);
                }

                response.Success = true;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Get example {id} failed: {ex}", id, ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ResultRes<ExampleNewRes>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveExample(ExampleNewReq request)
        {
            var response = new ResultRes<ExampleNewRes>();

            try
            {
                if (string.IsNullOrEmpty(request.Name))
                {
                    response.Error = "Name property is null or empty";
                    return BadRequest(response);
                }

                var result = await exampleService.Insert(Mapper.Map<ExampleDto>(request));

                response.Result = Mapper.Map<ExampleNewRes>(result);
                response.Success = true;

                return Ok(response); // 200
            }
            catch (Exception ex)
            {
                Logger.LogError("Save data unsuccessful: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }
    }
}
