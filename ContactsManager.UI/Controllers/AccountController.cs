using ContactsManager.Core.DTO;
using CRUD_Example.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            //TODO: register the user to the database

            return RedirectToAction(nameof(PersonsController.Index), "PersonsController");
        }
    }
}
