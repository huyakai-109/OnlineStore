using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Services.Admin;
using Training.Cms.Models;

namespace Training.Cms.Controllers
{
    [Authorize("AdminClerk")]
    public class ProductManagementController : Controller
    {
        private readonly IProductManagementService _productManagementService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductManagementController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductManagementController(IProductManagementService productManagementService, IMapper mapper, ILogger<ProductManagementController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _productManagementService = productManagementService;
            _mapper = mapper;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index([FromQuery] CommonSearchViewModel search)
        {
            var (products, pagination) = await _productManagementService.GetProducts(_mapper.Map<CommonSearchDto>(search));
            var productList = new CommonListViewModel<ProductViewModel>
            {
                Items = _mapper.Map<List<ProductViewModel>>(products),
                Pagination = pagination
            };
            return View(productList);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _productManagementService.GetCategories();
            var model = new ProductViewModel
            {
                Categories = _mapper.Map<List<CategoryDto>>(categories)
            };

            return PartialView("Create", model); 
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", productViewModel);
            }
            var productDto = _mapper.Map<ProductDto>(productViewModel);
            await _productManagementService.CreateProduct(productDto);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> AddImage(long productId)
        {
            //_logger.LogInformation($"Received productId: {productId}");
            var product = await _productManagementService.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductImageViewModel
            {
                ProductId = productId
            };
            return PartialView("AddImage", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddImage(ProductImageViewModel productImageVM)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = "Invalid data provided. Please check and try again.";
                return RedirectToAction("Error", new { message = errorMessage, controllerName = "ProductManagement" });
            }

            try
            {
                // Handle file upload for product images
                if (productImageVM.Image != null && productImageVM.Image.Length > 0)
                {
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileName = Path.GetFileName(productImageVM.Image.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productImageVM.Image.CopyToAsync(stream);
                    }

                    productImageVM.Path = $"images/products/{fileName}".Replace("\\", "/");
                }

                var productImageDto = _mapper.Map<ProductImageDto>(productImageVM);
                await _productManagementService.AddImage(productImageDto);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                var errorMessage = "An error occurred while processing your request. Please try again.";
                return RedirectToAction("Error", new { message = errorMessage, controllerName = "ProductManagement" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateThumbnail(long productId, string thumbnailPath)
        {
            await _productManagementService.UpdateThumbnail(productId, thumbnailPath);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var product = await _productManagementService.GetProductById(id);
          
            if (product == null)
            {
                return NotFound();
            }

            var productVM = _mapper.Map<ProductViewModel>(product);
            productVM.Categories = await _productManagementService.GetCategories(); 
            productVM.ProductImages = await _productManagementService.GetProductImages(productVM.Id);

            return PartialView("Edit", productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(long id, ProductViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = _mapper.Map<ProductDto>(model);
                    var result = await _productManagementService.UpdateProduct(product);

                    if (result) return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    var errorMessage = "Unable to save changes. The user was updated by another user.";
                    return RedirectToAction("Error", new { message = errorMessage, controllerName = "ProductManagement" });
                }

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _productManagementService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Error(string message, string controllerName)
        {
            ViewData["ErrorMessage"] = message;
            ViewData["ControllerName"] = controllerName;
            return View();
        }
    }
}
