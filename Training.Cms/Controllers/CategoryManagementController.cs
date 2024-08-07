using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        public CategoryManagementController(ICategoryManagementService categoryManagementService, IMapper mapper, ILogger<CategoryManagementController> logger)
        {
            _categoryManagementService = categoryManagementService;
            _mapper = mapper;
            _logger = logger;
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
                return View("Index", categoryViewModel);
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
                    var category = _mapper.Map<CategoryDto>(model);
                    var result = await _categoryManagementService.UpdateCategory(category);

                    if (result) return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {

                    ModelState.AddModelError(string.Empty, "Unable to save changes. The user was updated by another user.");
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
    }
}
