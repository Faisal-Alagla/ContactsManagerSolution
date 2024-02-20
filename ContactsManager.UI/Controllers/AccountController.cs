using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using CRUD_Example.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
	//Without the route attribute, conventional routing is applied (check program.cs)
	//[Route("[controller]/[action]")]
	[AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<ApplicationRole> _roleManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterDTO registerDTO)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);

				return View(registerDTO);
			}

			ApplicationUser user = new()
			{
				Email = registerDTO.Email,
				PersonName = registerDTO.PersonName,
				PhoneNumber = registerDTO.Phone,
				UserName = registerDTO.Email,
			};

			IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

			if (result.Succeeded)
			{
				if (registerDTO.UserType is UserTypeOptions.Admin)
				{
					//if the admin role doesn't exist in the db, create it
					if (await _roleManager.FindByIdAsync(UserTypeOptions.Admin.ToString()) is null)
					{
						ApplicationRole applicationRole = new() { Name = UserTypeOptions.Admin.ToString() };

						await _roleManager.CreateAsync(applicationRole);
					}

					//add the user into the admin role
					await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
				}
				else
				{
					//add the user into the user role
					await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
				}

				await _signInManager.SignInAsync(user, isPersistent: true);

				return RedirectToAction(nameof(PersonsController.Index), "Persons");
			}
			else
			{
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("Register", error.Description);
				}

				return View(registerDTO);
			}
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginDTO loginDTO, string? returnUrl)
		{
			if (ModelState.IsValid is false)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);

				return View(loginDTO);
			}

			var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: true, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
				{
					return LocalRedirect(returnUrl);
				}

				return RedirectToAction(nameof(PersonsController.Index), "Persons");
			}

			ModelState.AddModelError("Login", "Invalid user credentials");

			return View(loginDTO);
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction(nameof(PersonsController.Index), "PersonsController");
		}

		//used by the Remote Validation
		public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
		{
			ApplicationUser user = await _userManager.FindByEmailAsync(email);

			if (user is not null)
			{
				return Json(true);
			}
			return Json(false);
		}
	}
}
