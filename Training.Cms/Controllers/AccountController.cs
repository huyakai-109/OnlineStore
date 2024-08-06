using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Customers;
using Training.BusinessLogic.Services.Admin;
using Training.Cms.Models;
using Training.Common.EnumTypes;

namespace Training.Cms.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var userDto = _mapper.Map<UserDto>(loginVM);
                var user = await _userService.LoginAsync(userDto);

                if (user != null)
                {
                    // Redirect based on role
                    return RedirectToAction(user.Role == UserRole.Admin ? "Index" : "Index", user.Role == UserRole.Admin ? "Home" : "Home");
                }
                ModelState.AddModelError("", "Invalid email, password or access not allowed for this account.");
            }
            return View(loginVM);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out var userIdLong))
                {
                    return RedirectToAction("Login");
                }
                if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmPassword)
                {
                    ModelState.AddModelError("", "New password and confirm password do not match.");
                    return View(changePasswordViewModel);
                }

                var changePasswordDto = _mapper.Map<BusinessLogic.Dtos.Admin.ChangePasswordDto>(changePasswordViewModel);
                changePasswordDto.Id = userIdLong;

                var result = await _userService.ChangePasswordAsync(changePasswordDto);
                if (result)
                {
                    TempData["SuccessMessage"] = "Password has been changed successfully.";
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", "Invalid old password.");
            }
            return View(changePasswordViewModel);
        }

        [HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _userService.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet, ActionName("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out var userIdLong))
            {
                return RedirectToAction("Login");
            }
            var userDto = await _userService.GetProfile(userIdLong);
            if (userDto == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(_mapper.Map<UserViewModel>(userDto));
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
