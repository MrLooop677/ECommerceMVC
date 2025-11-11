using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        Repository<Category> _categoryRepository = new();

        //ApplicationDbContext _db = new();
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            //var categories = _db.Categories.AsNoTracking().AsQueryable();
            var categories = await _categoryRepository.GetAsync(tracked: false, cancellation: cancellationToken);
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View(new Category { });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Additional error.");
                return View(category);
            }
            //_db.Categories.Add(category);
            //_db.SaveChanges();
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id, CancellationToken cancellationToken)
        {
            //var category = _db.Categories.FirstOrDefault((e) => e.Id == id);
            var category = _categoryRepository.GetOneAsync((e) => e.Id == id, cancellation: cancellationToken);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category category, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            //_db.Categories.Update(category);
            //_db.SaveChanges();
            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            //var category = _db.Categories.FirstOrDefault((e) => e.Id == id);
            var category = await _categoryRepository.GetOneAsync((e) => e.Id == id, cancellation: cancellationToken);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            //_db.Categories.Remove(category);
            //_db.SaveChanges();
            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
