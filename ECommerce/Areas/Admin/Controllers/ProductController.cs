using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        ApplicationDbContext _db = new();
        public IActionResult Index()
        {
            var products = _db.Products.AsNoTracking().AsQueryable();
            products = products.Include((e) => e.Brand).Include((e) => e.Category);
            return View(products.AsAsyncEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product, IFormFile img, List<IFormFile> subImgs)
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
                product.MainImg = fileName;
            }
            else
            {

                // If no image provided, set a default image
                product.MainImg = "default-product.png";
            }
            // save product in db
            _db.Products.Add(product);
            _db.SaveChanges();
            if (subImgs is not null && subImgs.Count > 0)
            {
                foreach (var item in subImgs)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        // save img in db
                        item.CopyTo(stream);
                    }
                    _db.ProductSubImages.Add(new()
                    {
                        Product = product,
                        SubImg = fileName,
                        ProductId = product.Id,

                    });
                }
                _db.SaveChanges();
            }
            //send cookies to front end
            //Response.Cookies.Append("Notification", "add product succefully");
            //tempdData to send temp data when make refresh is deleted
            TempData["Notification"] = "add product succefully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _db.Products.FirstOrDefault((e) => e.Id == id);
            if (product is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? img)
        {
            var productInDb = _db.Products.FirstOrDefault(e => e.Id == product.Id);
            if (productInDb is null)
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
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", productInDb.MainImg);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                productInDb.MainImg = fileName;
            }
            // If no new image uploaded, keep the old image (no action needed)

            productInDb.Name = product.Name;
            productInDb.Status = product.Status;
            productInDb.Description = product.Description;

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _db.Products.FirstOrDefault((e) => e.Id == id);
            if (product is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            _db.Products.Remove(product);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}