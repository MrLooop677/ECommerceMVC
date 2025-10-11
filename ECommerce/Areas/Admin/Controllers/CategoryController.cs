using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        ApplicationDbContext _db = new();
        public IActionResult Index()
        {
            var categories = _db.Categories.AsNoTracking().AsQueryable();
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _db.Categories.FirstOrDefault((e) => e.Id == id);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _db.Categories.FirstOrDefault((e) => e.Id == id);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            _db.Categories.Remove(category);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
