using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
          if (_context.Categories == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
          }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (User.HasClaim("BrandId", id.ToString()) || User.IsInRole("BrandAdministrator"))
            {
                var category = _context.Categories!.FindAsync(id).Result;
                category!.StateId = 0;
                _context.Categories.Update(category);

                var foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                foreach (Food food in foods)
                {
                    food.StateId = 0;
                    _context.Foods.Update(food);
                }
                await _context.SaveChangesAsync();

                return Content($"{category.Name} alt katmanları silindi.");
            }
            return NotFound();
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public async Task<IActionResult> ChangeCategoryState(int id)
        {
            if (User.HasClaim("BrandId", id.ToString()) || User.IsInRole("RestaurantAdministrator"))
            {
                var category = _context.Categories!.FindAsync(id).Result;
                category!.StateId = 2;
                _context.Categories.Update(category);

                var foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                foreach (Food food in foods)
                {
                    food.StateId = 2;
                    _context.Foods.Update(food);
                }
                await _context.SaveChangesAsync();

                return Content($"{category.Name} alt katmanları kapatıldı/pasifleştirildi.");
            }
            return NotFound();
        }

        // POST: api/Categories/UploadImage/5
        [HttpPost("UploadImage/{id}")]
        [Authorize(Roles = "BrandAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Kategori bulunamadı.");
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

            category.Image = $"/images/{fileName}";
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla yüklendi.");
        }

        // PUT: api/Categories/EditImage/5
        [HttpPut("EditImage/{id}")]
        [Authorize(Roles = "BrandAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> EditImage(int id, IFormFile file)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Kategori bulunamadı.");
            }

            // Önce mevcut fotoğrafı silelim
            if (!string.IsNullOrEmpty(category.Image))
            {
                var imagePath = Path.Combine("wwwroot", category.Image.TrimStart('/'));
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

            category.Image = $"/images/{fileName}";
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla yüklendi.");
        }


        // DELETE: api/Categories/DeleteImage/5
        [HttpDelete("DeleteImage/{id}")]
        [Authorize(Roles = "BrandAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> DeleteImage(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Kategori bulunamadı.");
            }

            if (string.IsNullOrEmpty(category.Image))
            {
                return BadRequest("Kategorinin bir görseli bulunmuyor.");
            }

            var imagePath = Path.Combine("wwwroot", category.Image.TrimStart('/'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            category.Image = null;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla silindi.");
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
