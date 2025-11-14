using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IProductRepository productRepository; // = new ProductRepository();
        private readonly IRepository<Category> categoryRepository; // = new Repository<Category>();
        private readonly IRepository<Brand> brandRepository;// = new Repository<Brand>();
        private readonly IRepository<ProductSubImage> productSubImage;// = new Repository<ProductSubImage>();
        private readonly IProductColorRepository productColor;// = new ProductColorRepository();

        public ProductController(ApplicationDbContext db, IProductRepository productRepository, IRepository<Category> categoryRepository, IRepository<Brand> brandRepository, IRepository<ProductSubImage> productSubImage, IProductColorRepository productColor)
        {
            _db = db;
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.brandRepository = brandRepository;
            this.productSubImage = productSubImage;
            this.productColor = productColor;
        }

        //private readonly //Repository<ProductColor> productColor = new Repository<ProductColor>();


        public async Task<IActionResult> Index(FilterProductVM filterProductVM, CancellationToken cancellationToken, int page = 1, int discount = 0)
        {
            //var products = _db.Products.AsNoTracking().AsQueryable();
            //products = products.Include((e) => e.Brand).Include((e) => e.Category);
            var products = await productRepository.GetAsync(includes: [e => e.Brand, e => e.Category], cancellation: cancellationToken, tracked: false);
            #region Filter Product
            // Add Filter 
            if (filterProductVM.name is not null)
            {
                //products = products.Where(e => e.Name.Contains(filterProductVM.name.Trim()));
                products = await productRepository.GetAsync(e => e.Name.Contains(filterProductVM.name.Trim()), cancellation: cancellationToken);
                ViewBag.name = filterProductVM.name;
            }

            if (filterProductVM.minPrice is not null)
            {
                //products = products.Where(e => e.price - e.price * e.Discount / 100 > filterProductVM.minPrice);
                products = await productRepository.GetAsync(e => e.price - e.price * e.Discount / 100 > filterProductVM.minPrice, cancellation: cancellationToken);
                ViewBag.minPrice = filterProductVM.minPrice;
            }

            if (filterProductVM.maxPrice is not null)
            {
                //products = products.Where(e => e.price - e.price * e.Discount / 100 < filterProductVM.maxPrice);
                products = await productRepository.GetAsync(e => e.price - e.price * e.Discount / 100 < filterProductVM.maxPrice, cancellation: cancellationToken);
                ViewBag.maxPrice = filterProductVM.maxPrice;
            }

            if (filterProductVM.categoryId is not null)
            {
                //products = products.Where(e => e.CategoryId == filterProductVM.categoryId);
                products = await productRepository.GetAsync(e => e.CategoryId == filterProductVM.categoryId, cancellation: cancellationToken);
                ViewBag.categoryId = filterProductVM.categoryId;
            }

            if (filterProductVM.brandId is not null)
            {
                //products = products.Where(e => e.BrandId == filterProductVM.brandId);
                products = await productRepository.GetAsync(e => e.BrandId == filterProductVM.brandId, cancellation: cancellationToken);
                ViewBag.brandId = filterProductVM.brandId;
            }

            if (filterProductVM.LessQuantity)
            {
                products = products.OrderBy(e => e.Quantity);
                ViewBag.LessQuantity = filterProductVM.LessQuantity;
            }

            // Categories
            var categories = await categoryRepository.GetAsync(cancellation: cancellationToken);
            ViewBag.categories = categories.AsEnumerable();

            // Brands
            var brands = await brandRepository.GetAsync(cancellation: cancellationToken);
            ViewData["brands"] = brands.AsEnumerable();
            #endregion

            #region Pagination
            // Pagination
            ViewBag.TotalPages = Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = page;
            products = products.Skip((page - 1) * 8).Take(8); // 0 .. 8 
            #endregion
            return View(products.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {

            // Categories
            var categories = await categoryRepository.GetAsync(cancellation: cancellationToken);
            // Brands
            var brands = await brandRepository.GetAsync(cancellation: cancellationToken);

            return View(new ProductVM
            {
                Categories = categories.AsEnumerable(),
                Brands = brands.AsEnumerable(),
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile img, List<IFormFile> subImgs, string[] colors, CancellationToken cancellationToken)
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
                //_db.Products.Add(product);
                //_db.SaveChanges();
                await productRepository.AddAsync(product, cancellationToken);
                await productRepository.CommitAsync(cancellationToken);

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

                        //_db.ProductSubImages.Add(new ProductSubImage()
                        //{
                        //    ProductId = product.Id,
                        //    SubImg = fileName,
                        //});
                        //_db.SaveChanges();
                        await productSubImage.AddAsync(new ProductSubImage()
                        {
                            ProductId = product.Id,
                            SubImg = fileName,
                        }, cancellation: cancellationToken);
                        await productSubImage.CommitAsync(cancellationToken);
                    }
                }

                // colors
                if (colors.Any())
                {
                    foreach (var item in colors)
                    {
                        await productColor.AddAsync(new ProductColor()
                        {
                            Color = item,
                            ProductId = product.Id,
                        }, cancellation: cancellationToken);
                    }
                    await productColor.CommitAsync(cancellationToken);
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
        public async Task<IActionResult> Edit(int id)
        {
            //var product = _db.Products.Include(e => e.ProductColors).Include(e => e.ProductSubImages).FirstOrDefault(e => e.Id == id);
            var product = await productRepository.GetOneAsync(e => e.Id == id, includes: [e => e.ProductColors, e => e.ProductSubImages]);

            if (product is null)
                return RedirectToAction("NotFoundPage", "Home");

            // Categories
            //var categories = _db.Categories;
            var categories = await categoryRepository.GetAsync();
            // Brands
            //var brands = _db.Brands;
            var brands = await brandRepository.GetAsync();

            return View(new ProductVM
            {
                Categories = categories.AsEnumerable(),
                Brands = brands.AsEnumerable(),
                Product = product,
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? img, List<IFormFile> subImgs, string[] colors, CancellationToken cancellationToken)
        {
            //var productInDb = _db.Products.Include(e => e.ProductColors).FirstOrDefault(e => e.Id == product.Id);
            var productInDb = await productRepository.GetOneAsync(e => e.Id == product.Id, includes: [e => e.ProductColors]);
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

            await productRepository.CommitAsync(cancellation: cancellationToken);
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

                    //_db.ProductSubImages.Add(new ProductSubImage()
                    //{
                    //    ProductId = product.Id,
                    //    SubImg = fileName,
                    //});
                    await productSubImage.AddAsync(new ProductSubImage()
                    {
                        ProductId = product.Id,
                        SubImg = fileName,
                    }, cancellation: cancellationToken);
                }
                await productSubImage.CommitAsync(cancellation: cancellationToken);
            }
            if (colors.Any())
            {

                //_db.ProductColors.RemoveRange(productInDb.ProductColors);
                productColor.RemoveRange(productInDb.ProductColors);

                foreach (var item in colors)
                {
                    //_db.ProductColors.Add(new ProductColor()
                    //{
                    //    Color = item,
                    //    ProductId = product.Id,
                    //});
                    await productColor.AddAsync(new ProductColor()
                    {
                        Color = item,
                        ProductId = product.Id,
                    }, cancellation: cancellationToken);
                }
                //_db.SaveChanges();
                await productColor.CommitAsync(cancellation: cancellationToken);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            //var product = _db.Products.Include(e => e.ProductColors).Include(e => e.ProductSubImages).FirstOrDefault(e => e.Id == id);
            var product = await productRepository.GetOneAsync(e => e.Id == id, includes: [e => e.ProductColors, e => e.ProductSubImages]);

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


            //_db.Products.Remove(product);
            //_db.SaveChanges();
            productRepository.Delete(product);
            await productRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Delete Product Successfully";

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteSubImg(int productId, string Img, CancellationToken cancellationToken)
        {
            //var productSubImg = _db.ProductSubImages.FirstOrDefault(e => e.ProductId == productId && e.SubImg == Img);
            var productSubImg = await productSubImage.GetOneAsync(e => e.ProductId == productId && e.SubImg == Img, cancellation: cancellationToken);
            if (productSubImg is null)
                return RedirectToAction("NotFound", "Home");
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", productSubImg.SubImg);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            //_db.ProductSubImages.Remove(productSubImg);
            //_db.SaveChanges();
            productSubImage.Delete(productSubImg);
            await productSubImage.CommitAsync(cancellation: cancellationToken);
            return (RedirectToAction(nameof(Edit), new { id = productId }));

        }
    }
}