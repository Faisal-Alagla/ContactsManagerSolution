using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] //can applied to action methods as well
    public class HomeController : Controller
    {
        //[Route("admin/home/index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
