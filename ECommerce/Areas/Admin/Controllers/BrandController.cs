using ECommerce.ViewModels;
using Mapster;
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

            return View(new CreateBrandRequest());
        }
        [HttpPost]
        public IActionResult Create(CreateBrandRequest createBrandRequest)
        {
            if (!ModelState.IsValid) { return View(createBrandRequest); }
            //Brand brand = new()
            //{
            //    Name = createBrandRequest.Name,
            //    Description = createBrandRequest.Description,
            //    Status = createBrandRequest.Status
            //};
            Brand brand = createBrandRequest.Adapt<Brand>();
            if (createBrandRequest.Img is not null && createBrandRequest.Img.Length > 0)
            {
                // save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createBrandRequest.Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                //using is to make destroyed after finsished method 
                using (var stream = System.IO.File.Create(filePath))
                {
                    // save img in db
                    createBrandRequest.Img.CopyTo(stream);
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
            //send cookies to front end
            //Response.Cookies.Append("Notification", "add brand succefully");
            //tempdData to send temp data when make refresh is deleted
            TempData["Notification"] = "add brand succefully";
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
            //return View(new UpdateBrandRequest()
            //{
            //    Id = brand.Id,
            //    Name = brand.Name,
            //    Description = brand.Description,
            //    Status = brand.Status,
            //    Img = brand.Img


            //});
            return View(brand.Adapt<UpdateBrandRequest>());
        }
        [HttpPost]
        public IActionResult Edit(UpdateBrandRequest updateBrandRequest)
        {
            if (!ModelState.IsValid) { return View(updateBrandRequest); }
            //Brand brand = new()
            //{
            //    Id = updateBrandRequest.Id,
            //    Name = updateBrandRequest.Name,
            //    Description = updateBrandRequest.Description,
            //    Status = updateBrandRequest.Status
            //};
            Brand brand = updateBrandRequest.Adapt<Brand>();

            var brandInDb = _db.Brands.FirstOrDefault(e => e.Id == brand.Id);
            if (brandInDb is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            if (updateBrandRequest.NewImg is not null && updateBrandRequest.NewImg.Length > 0)
            {
                // save img in wwwroot
                // craete name img with extension
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateBrandRequest.NewImg.FileName);
                // put img in path  "wwwroot\\images"
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                //using is to make destroyed after finsished method 
                using (var stream = System.IO.File.Create(filePath))
                {
                    // save img in db
                    updateBrandRequest.NewImg.CopyTo(stream);
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