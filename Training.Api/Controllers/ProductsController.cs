using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Models.Responses.Base;
using Training.Api.Models.Responses.Examples;
using Training.Api.Models.Responses.Products;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Dtos.Customers;
using Training.BusinessLogic.Services;


namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController(ILogger<ProductsController> logger,
        IMapper mapper,
        ICustomerProductService customerProductService) : BaseController(logger, mapper)
    {
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultRes<ProductRes?>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultRes<ProductRes?>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductDetails(long id)
        {
            var response = new ResultRes<ProductRes?>();
            try
            {

                response.Result = Mapper.Map<ProductRes>(await customerProductService.GetProductByIdAsync(id));

                if (response.Result == null)
                {
                    response.Error = $"Unable to find product with Id: {id}";
                    return NotFound(response);
                }

                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Get product details {id} failed: {ex}", id, ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResultRes<List<ProductRes>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts([FromQuery]SearchDto searchDto)
        {
            var response = new PaginationResultRes<List<ProductRes>>();
            try
            {
                var products = await customerProductService.GetProducts(searchDto);
                response.Result = Mapper.Map<List<ProductRes>>(products.Items);
                response.Pagination = new PaginationRes(products.TotalCount, products.CurrentCount, searchDto.Skip, searchDto.Take);
                response.Success = true;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Get products failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }
        [HttpGet("search")]
        [ProducesResponseType(typeof(PaginationResultRes<List<ProductRes>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchDto searchDto)

        {
            var response = new PaginationResultRes<List<ProductRes>>();
            try
            {
                var products = await customerProductService.SearchProducts(searchDto);
                response.Result = Mapper.Map<List<ProductRes>>(products.Items);
                response.Pagination = new PaginationRes(products.TotalCount, products.CurrentCount, searchDto.Skip, searchDto.Take);
                response.Success = true;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Search products failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }
    }
}
