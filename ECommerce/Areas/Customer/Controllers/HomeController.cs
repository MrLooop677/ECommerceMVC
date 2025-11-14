using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _db;//= new();

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _db = context;
            _logger = logger;
        }

        public IActionResult Index(FilterProductVM filterProductVM, int page = 1)
        {
            decimal discount = 80;

            var products = _db.Products.AsNoTracking().Include(c => c.Category).AsQueryable();
            if (filterProductVM.name is not null)
            {
                products = products.Where(p => p.Name.Contains(filterProductVM.name));
                ViewBag.name = filterProductVM.name;


            }

            if (filterProductVM.minPrice is not null)
            {
                products = products.Where(p => p.price - p.price * p.Discount / 100 > filterProductVM.minPrice);
                ViewBag.minPrice = filterProductVM.minPrice;
            }
            if (filterProductVM.minPrice is not null)
            {
                products = products.Where(p => p.price - p.price * p.Discount / 100 < filterProductVM.maxPrice);
                ViewBag.maxPrice = filterProductVM.maxPrice;
            }
            if (filterProductVM.categoryId is not null)
            {
                products = products.Where(p => p.CategoryId == filterProductVM.categoryId);
                ViewBag.categoryId = filterProductVM.categoryId;

            }
            if (filterProductVM.brandId is not null)
            {
                products = products.Where(p => p.BrandId == filterProductVM.brandId);
                ViewBag.brandId = filterProductVM.brandId;
            }
            if (filterProductVM.isHot)
            {
                products = products.Where(p => p.Discount > discount);
                ViewBag.isHot = filterProductVM.isHot;
            }
            var categories = _db.Categories;
            ViewData["categories"] = categories.AsEnumerable();
            var brands = _db.Brands;
            ViewBag.brands = brands.AsEnumerable();
            ViewBag.totalPage = Math.Ceiling(products.Count() / 8.0);
            ViewBag.currentPage = page;
            products = products.Skip((page - 1) * 8).Take(8);
            return View(products.AsNoTracking().AsEnumerable());

        }
        public IActionResult Privacy()
        {
            return View();
        }
        public ViewResult Welcome()
        {
            return View();
        }
        public ViewResult PersonalInfo()
        {
            List<string> names = new List<string>()
            {
                "Alice",
                "Bob",
                "Charlie",
                "Diana"
            };
            return View(names);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
