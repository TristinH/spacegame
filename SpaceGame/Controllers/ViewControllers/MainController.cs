using Microsoft.AspNetCore.Mvc;

namespace SpaceGame.Controllers.ViewControllers
{
    public class MainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
