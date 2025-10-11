using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        ApplicationDbContext _db = new();
        public IActionResult Index()
        {
            var brands = _db.Brands.AsNoTracking().AsQueryable();
            return View(brands.Select(e => new
            {
                e.Id,
                e.Name,
                e.Description,
                e.Status,
                e.Img
            }).AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand brand, IFormFile img)
        {
            if (img is not null && img.Length > 0)
            {
                // save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                //using is to make destroyed after finsished method 
                using (var stream = System.IO.File.Create(filePath))
                {
                    // save img in db
                    img.CopyTo(stream);
                }
                brand.Img = fileName;
            }
            else
            {

                // If no image provided, set a default image
                brand.Img = "default-brand.png";
            }
            // save brand in db
            _db.Brands.Add(brand);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var brand = _db.Brands.FirstOrDefault((e) => e.Id == id);
            if (brand is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(brand);
        }
        [HttpPost]
        public IActionResult Edit(Brand brand, IFormFile? img)
        {
            var brandInDb = _db.Brands.FirstOrDefault(e => e.Id == brand.Id);
            Console.WriteLine(brandInDb);
            if (brandInDb is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            if (img is not null && img.Length > 0)
            {
                // save img in wwwroot
                // craete name img with extension
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                // put img in path  "wwwroot\\images"
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                //using is to make destroyed after finsished method 
                using (var stream = System.IO.File.Create(filePath))
                {
                    // save img in db
                    img.CopyTo(stream);
                }

                //Remove old img from folder
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brandInDb.Img);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                brandInDb.Img = fileName;
            }
            // If no new image uploaded, keep the old image (no action needed)

            brandInDb.Name = brand.Name;
            brandInDb.Status = brand.Status;
            brandInDb.Description = brand.Description;

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var brand = _db.Brands.FirstOrDefault((e) => e.Id == id);
            if (brand is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            _db.Brands.Remove(brand);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}