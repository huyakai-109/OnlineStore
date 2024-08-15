using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Services.Admin;
using Training.Cms.Models;
using Training.Common.Constants;

namespace Training.Cms.Controllers
{
    [Authorize(Policy = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(IUserManagementService userManagementService, IMapper mapper, ILogger<UserManagementController> logger)
        {
            _userManagementService = userManagementService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IActionResult> Index([FromQuery] CommonSearchViewModel search)
        {
            var (users, pagination) = await _userManagementService.GetUsers(_mapper.Map<CommonSearchDto>(search));
            var userList = new CommonListViewModel<UserViewModel>
            {
                Items = _mapper.Map<List<UserViewModel>>(users),
                Pagination = pagination
            };
            
            return View(userList);
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            //_logger.LogInformation($"Received user: {JsonConvert.SerializeObject(userViewModel)}");
            if (!ModelState.IsValid)
            {

                //_logger.LogWarning("ModelState is invalid.");
                //foreach (var state in ModelState)
                //{
                //    foreach (var error in state.Value.Errors)
                //    {
                //        _logger.LogWarning($"Key: {state.Key}, Error: {error.ErrorMessage}");
                //    }
                //}
                return View("Index", userViewModel);
            }
            var userDto = _mapper.Map<UserDto>(userViewModel);
            await _userManagementService.CreateUser(userDto);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var user = await _userManagementService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var userVM = _mapper.Map<UserViewModel>(user);
            return PartialView("Edit", userVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long id, UserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _mapper.Map<UserDto>(model);
                    var result = await _userManagementService.UpdateUser(user);

                    if (result) return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    var errorMessage = "Unable to save changes. The user was updated by another user.";
                    return RedirectToAction("Error", new { message = errorMessage, controllerName = "UserManagement" });
                }

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _userManagementService.DeleteUser(id); 
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            var users = await _userManagementService.GetAllUsers();  
            var csvFile = _userManagementService.ExportUsersToCsv(users);
            return File(csvFile, GlobalConstants.Files.CsvContentType, "Users.csv");
        }

        public IActionResult Error(string message, string controllerName)
        {
            ViewData["ErrorMessage"] = message;
            ViewData["ControllerName"] = controllerName;
            return View();
        }
    }
}
