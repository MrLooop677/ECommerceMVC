using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        ApplicationDbContext _db = new();
        public IActionResult Index(FilterProductVM filterProductVM, int page = 1, int discount = 0)
        {
            var products = _db.Products.AsNoTracking().AsQueryable();
            products = products.Include((e) => e.Brand).Include((e) => e.Category);
            #region Filter Product
            // Add Filter 
            if (filterProductVM.name is not null)
            {
                products = products.Where(e => e.Name.Contains(filterProductVM.name.Trim()));
                ViewBag.name = filterProductVM.name;
            }

            if (filterProductVM.minPrice is not null)
            {
                products = products.Where(e => e.price - e.price * e.Discount / 100 > filterProductVM.minPrice);
                ViewBag.minPrice = filterProductVM.minPrice;
            }

            if (filterProductVM.maxPrice is not null)
            {
                products = products.Where(e => e.price - e.price * e.Discount / 100 < filterProductVM.maxPrice);
                ViewBag.maxPrice = filterProductVM.maxPrice;
            }

            if (filterProductVM.categoryId is not null)
            {
                products = products.Where(e => e.CategoryId == filterProductVM.categoryId);
                ViewBag.categoryId = filterProductVM.categoryId;
            }

            if (filterProductVM.brandId is not null)
            {
                products = products.Where(e => e.BrandId == filterProductVM.brandId);
                ViewBag.brandId = filterProductVM.brandId;
            }

            if (filterProductVM.LessQuantity)
            {
                products = products.OrderBy(e => e.Quantity);
                ViewBag.LessQuantity = filterProductVM.LessQuantity;
            }

            // Categories
            var categories = _db.Categories;
            ViewBag.categories = categories.AsEnumerable();

            // Brands
            var brands = _db.Brands;
            ViewData["brands"] = brands.AsEnumerable();
            #endregion

            #region Pagination
            // Pagination
            ViewBag.TotalPages = Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = page;
            products = products.Skip((page - 1) * 8).Take(8); // 0 .. 8 
            #endregion
            return View(products.AsAsyncEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {

            // Categories
            var categories = _db.Categories;
            // Brands
            var brands = _db.Brands;

            return View(new ProductVM
            {
                Categories = categories.AsEnumerable(),
                Brands = brands.AsEnumerable(),
            });
        }
        [HttpPost]
        public IActionResult Create(Product product, IFormFile img, List<IFormFile> subImgs, string[] colors)
        {
            var transAction = _db.Database.BeginTransaction();
            try
            {
                // main image
                if (img is not null && img.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        img.CopyTo(stream);
                    }
                    product.MainImg = fileName;
                }
                else
                {
                    product.MainImg = "default-product.png";
                }

                // save product
                _db.Products.Add(product);
                _db.SaveChanges();

                // sub images
                if (subImgs is not null && subImgs.Count > 0)
                {
                    foreach (var item in subImgs)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            item.CopyTo(stream);
                        }

                        _db.ProductSubImages.Add(new ProductSubImage()
                        {
                            ProductId = product.Id,
                            SubImg = fileName,
                        });
                    }
                    _db.SaveChanges();
                }

                // colors
                if (colors.Any())
                {
                    foreach (var item in colors)
                    {
                        _db.ProductColors.Add(new ProductColor()
                        {
                            Color = item,
                            ProductId = product.Id,
                        });
                    }
                    _db.SaveChanges();
                }

                transAction.Commit(); // ✅ ضروري
                TempData["Notification"] = "add product successfully";
            }
            catch (Exception ex)
            {
                transAction.Rollback(); // ✅ rollback في حالة الخطأ
                TempData["err-Notification"] = "Error While Saving The Product";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _db.Products.Include(e => e.ProductColors).Include(e => e.ProductSubImages).FirstOrDefault(e => e.Id == id);

            if (product is null)
                return RedirectToAction("NotFoundPage", "Home");

            // Categories
            var categories = _db.Categories;
            // Brands
            var brands = _db.Brands;

            return View(new ProductVM
            {
                Categories = categories.AsEnumerable(),
                Brands = brands.AsEnumerable(),
                Product = product,
            });
        }
        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? img, List<IFormFile> subImgs, string[] colors)
        {
            var productInDb = _db.Products.Include(e => e.ProductColors).FirstOrDefault(e => e.Id == product.Id);
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
            if (subImgs is not null && subImgs.Count > 0)
            {
                foreach (var item in subImgs)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    _db.ProductSubImages.Add(new ProductSubImage()
                    {
                        ProductId = product.Id,
                        SubImg = fileName,
                    });
                }
                _db.SaveChanges();
            }
            if (colors.Any())
            {

                _db.ProductColors.RemoveRange(productInDb.ProductColors);

                foreach (var item in colors)
                {
                    _db.ProductColors.Add(new ProductColor()
                    {
                        Color = item,
                        ProductId = product.Id,
                    });
                }
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _db.Products.Include(e => e.ProductColors).Include(e => e.ProductSubImages).FirstOrDefault(e => e.Id == id);

            if (product is null)
                return RedirectToAction("NotFoundPage", "Home");

            // Remove old Img in wwwroot
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            foreach (var item in product.ProductSubImages)
            {
                var subImgOldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", item.SubImg);
                if (System.IO.File.Exists(subImgOldPath))
                {
                    System.IO.File.Delete(subImgOldPath);
                }
            }


            _db.Products.Remove(product);
            _db.SaveChanges();

            TempData["success-notification"] = "Delete Product Successfully";

            return RedirectToAction(nameof(Index));
        }


        public IActionResult DeleteSubImg(int productId, string Img)
        {
            var productSubImg = _db.ProductSubImages.FirstOrDefault(e => e.ProductId == productId && e.SubImg == Img);
            if (productSubImg is null)
                return RedirectToAction("NotFound", "Home");
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", productSubImg.SubImg);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _db.ProductSubImages.Remove(productSubImg);
            _db.SaveChanges();
            return (RedirectToAction(nameof(Edit), new { id = productId }));

        }
    }
}