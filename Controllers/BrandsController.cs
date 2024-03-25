using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BrandsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Brands
        [HttpGet]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
        {
          if (_context.Brands == null)
          {
              return NotFound();
          }
            return await _context.Brands.ToListAsync();
        }

        // GET: api/Brands/5
        [HttpGet("{id}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
          if (_context.Brands == null)
          {
              return NotFound();
          }
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        // PUT: api/Brands/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> PutBrand(int id, Brand brand)
        {
            if (User.HasClaim("Restaurant", id.ToString()) || User.IsInRole("BrandAdministrator"))
            {
                _context.Entry(brand).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            if (id != brand.Id)
            {
                return BadRequest("Bu kimliğe sahip bir restoran yok.");
            }
            return Ok($"{brand.Name} güncellendi.");
        }

        // POST: api/Brands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<ActionResult<Brand>> PostBrand(Brand brand, string userName, string password)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            Claim claim;

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            applicationUser.Name = brand.Name;
            applicationUser.PhoneNumber = brand.Phone;
            applicationUser.CompanyId = brand.CompanyId;
            applicationUser.StateId = 1;
            applicationUser.UserName = userName;

            var result = _userManager.CreateAsync(applicationUser, password).Result;
            claim = new Claim("Brand", brand.Id.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();

            _userManager.AddToRoleAsync(applicationUser, "BrandAdministrator").Wait();

            return Ok($"{brand.Name} güncellendi.");
        }

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> DeleteBrand(int id)
        {

            if (User.HasClaim("BrandId", id.ToString()) || User.IsInRole("CompanyAdministrator"))
            {
                var brand = _context.Brands!.FindAsync(id).Result;
                brand!.StateId = 0;
                _context.Brands.Update(brand);

                var restaurants = _context.Restaurants.Where(r => r.BrandId == id);
                foreach (Restaurant restaurant in restaurants)
                {
                    restaurant.StateId = 0;
                    _context.Restaurants.Update(restaurant);

                    var categories = _context.Categories.Where(c => c.RestaurantId == restaurant.Id);
                    foreach (Category category in categories)
                    {
                        category.StateId = 0;
                        _context.Categories.Update(category);

                        var foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                        foreach (Food food in foods)
                        {
                            food.StateId = 0;
                            _context.Foods.Update(food);
                        }
                    }
                }
                await _context.SaveChangesAsync();

                return Content($"{brand.Name} alt katmanları silindi.");
            }
            return NotFound();
        }

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<IActionResult> ChangeBrandState(int id)
        {

            if (User.HasClaim("BrandId", id.ToString()) || User.IsInRole("CompanyAdministrator"))
            {
                var brand = _context.Brands!.FindAsync(id).Result;
                brand!.StateId = 2;
                _context.Brands.Update(brand);

                var restaurants = _context.Restaurants.Where(r => r.BrandId == id);
                foreach (Restaurant restaurant in restaurants)
                {
                    restaurant.StateId = 2;
                    _context.Restaurants.Update(restaurant);

                    var categories = _context.Categories.Where(c => c.RestaurantId == restaurant.Id);
                    foreach (Category category in categories)
                    {
                        category.StateId = 2;
                        _context.Categories.Update(category);

                        var foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                        foreach (Food food in foods)
                        {
                            food.StateId = 2;
                            _context.Foods.Update(food);
                        }
                    }
                }
                await _context.SaveChangesAsync();

                return Content($"{brand.Name} alt katmanları kapatıldı/pasifleştirildi.");
            }
            return NotFound();
        }

        // POST: api/Brands/UploadImage/5
        [HttpPost("UploadImage/{id}")]
        [Authorize(Roles = "CompanyAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound("Marka bulunamadı.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Lütfen resim dosyası seçiniz.");
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest("Lütfen resim dosyası seçiniz.");
            }

            var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine("wwwroot", "images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            brand.Logo = $"/images/{fileName}";
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla yüklendi.");
        }

        // PUT: api/Brands/EditImage/5
        [HttpPut("EditImage/{id}")]
        [Authorize(Roles = "CompanyAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> EditImage(int id, IFormFile file)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound("Marka bulunamadı.");
            }

            // Önce mevcut fotoğrafı silelim
            if (!string.IsNullOrEmpty(brand.Logo))
            {
                var imagePath = Path.Combine("wwwroot", brand.Logo.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Ardından yeni fotoğrafı yükleyelim
            if (file == null || file.Length == 0)
            {
                return BadRequest("Lütfen resim dosyası seçiniz.");
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest("Lütfen resim dosyası seçiniz.");
            }

            var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine("wwwroot", "images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            brand.Logo = $"/images/{fileName}";
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla yüklendi.");
        }


        // DELETE: api/Brands/DeleteImage/5
        [HttpDelete("DeleteImage/{id}")]
        [Authorize(Roles = "CompanyAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> DeleteImage(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound("Marka bulunamadı.");
            }

            if (string.IsNullOrEmpty(brand.Logo))
            {
                return BadRequest("Markanın bir görseli bulunmuyor.");
            }

            var imagePath = Path.Combine("wwwroot", brand.Logo.TrimStart('/'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            brand.Logo = null;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla silindi.");
        }


        private bool BrandExists(int id)
        {
            return (_context.Brands?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
