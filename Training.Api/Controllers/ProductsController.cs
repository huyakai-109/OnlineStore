using AutoMapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Training.Api.Models.Requests.Carts;
using Training.Api.Models.Requests.Products;
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
        public async Task<IActionResult> GetProducts([FromQuery] SearchReq searchReq)
        {
            var response = new PaginationResultRes<List<ProductRes>>();
            try
            {

                var products = await customerProductService.GetProducts(Mapper.Map<SearchDto>(searchReq));
                response.Result = Mapper.Map<List<ProductRes>>(products.Items);
                response.Pagination = new PaginationRes(products.TotalCount, products.CurrentCount, searchReq.Skip, searchReq.Take);
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
        public async Task<IActionResult> SearchProducts([FromQuery] SearchReq searchReq)

        {
            var response = new PaginationResultRes<List<ProductRes>>();
            try
            {
                var products = await customerProductService.SearchProducts(Mapper.Map<ProductSearchDto>(searchReq));
                response.Result = Mapper.Map<List<ProductRes>>(products.Items);
                response.Pagination = new PaginationRes(products.TotalCount, products.CurrentCount, searchReq.Skip, searchReq.Take);
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

        [HttpPost("add-to-cart")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartReq addToCartReq)
        {
            var response = new ResultRes<bool>();
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null)
                {
                    response.Error = "User ID not found";
                    return BadRequest(response);
                }
                var userId = long.Parse(userIdClaim.Value);

                var addToCartDto = Mapper.Map<AddToCartDto>(addToCartReq);
                addToCartDto.UserId = userId;
                var result = await customerProductService.AddToCartAsync(addToCartDto);

                if (!result)
                {
                    response.Error = "Failed to add product to cart";
                    return BadRequest(response);
                }

                response.Success = true;
                response.Result = true;
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError("Add products to cart failed: {ex}", ex);
                response.Success = false;
                response.Error = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Add products to cart failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
            
        }
    }
}
