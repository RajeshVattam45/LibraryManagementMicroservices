using Microsoft.AspNetCore.Mvc;

namespace BookCatalogService.API.Controllers
{
    public class BooksController : Controller
    {
       
        public IActionResult Index ( )
        {
            return View ();
        }
    }
}
