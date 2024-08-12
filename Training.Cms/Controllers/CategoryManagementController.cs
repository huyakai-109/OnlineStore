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
    [Authorize(Policy = "AdminClerk")]
    public class CategoryManagementController : Controller
    {
        private readonly ICategoryManagementService _categoryManagementService;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryManagementController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryManagementController(ICategoryManagementService categoryManagementService, IMapper mapper, ILogger<CategoryManagementController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _categoryManagementService = categoryManagementService;
            _mapper = mapper;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index([FromQuery] CommonSearchViewModel search)
        {
            var (categories, pagination) = await _categoryManagementService.GetCategories(_mapper.Map<CommonSearchDto>(search));
            var categoryList = new CommonListViewModel<CategoryViewModel>
            {
                Items = _mapper.Map<List<CategoryViewModel>>(categories),
                Pagination = pagination
            };

            return View(categoryList);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = "Invalid data provided. Please check and try again.";
                return RedirectToAction("Error", new { message = errorMessage, controllerName = "CategoryManagement" });
            }

            // Handle file upload
            if (categoryViewModel.Image != null && categoryViewModel.Image.Length > 0)
            {
                // Ensure directory exists
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/categories");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Save the file
                var fileName = Path.GetFileName(categoryViewModel.Image.FileName);
                var filePath = Path.Combine(uploadPath, fileName);  

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await categoryViewModel.Image.CopyToAsync(stream);
                }

                // Set the image path for database storage
                categoryViewModel.ImagePath = $"images/categories/{fileName}".Replace("\\", "/");
            }

            var categoryDto = _mapper.Map<CategoryDto>(categoryViewModel);
            await _categoryManagementService.CreateCategory(categoryDto);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var category = await _categoryManagementService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = _mapper.Map<CategoryViewModel>(category);
            return PartialView("Edit", categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(long id, CategoryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Image != null && model.Image.Length > 0)
                    {
                        var fileName = Path.GetFileName(model.Image.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/categories", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(stream);
                        }

                        model.ImagePath = $"images/categories/{fileName}".Replace("\\", "/");
                    }

                    var category = _mapper.Map<CategoryDto>(model);
                    var result = await _categoryManagementService.UpdateCategory(category);

                    if (result) return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    var errorMessage = "Unable to save changes. The category was updated by another user.";
                    return RedirectToAction("Error", new { message = errorMessage, controllerName = "CategoryManagement" });
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _categoryManagementService.DeleteCategory(id);
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
