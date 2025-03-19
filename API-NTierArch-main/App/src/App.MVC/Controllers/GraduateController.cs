using Microsoft.AspNetCore.Mvc;

namespace App.MVC.Controllers
{
    public class GraduateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
