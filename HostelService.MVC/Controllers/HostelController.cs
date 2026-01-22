using Microsoft.AspNetCore.Mvc;

namespace HostelService.MVC.Controllers
{
    public class HostelController : Controller
    {
        public IActionResult Index ( )
        {
            return View ();
        }
    }
}
