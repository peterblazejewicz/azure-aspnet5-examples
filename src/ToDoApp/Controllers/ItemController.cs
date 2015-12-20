using Microsoft.AspNet.Mvc;
using ToDoApp.Models;
using ToDoApp.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoApp.Controllers
{
    public class ItemController : Controller
    {
        public ItemController(IDocumentDBRepository<Item> db)
        {
            this.db = db;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var items = db.GetItems(d => !d.Completed);
            return View(items);
        }

        private IDocumentDBRepository<Item> db;
    }
}
