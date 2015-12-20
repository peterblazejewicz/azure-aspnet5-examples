using Microsoft.AspNet.Mvc;
using ToDoApp.Services;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    public class HomeController : Controller
    {

        public HomeController(IDocumentDBRepository<Item> db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        private IDocumentDBRepository<Item> db;
    }
}
