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
    public class FoodsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FoodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Foods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFoods()
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
            return await _context.Foods.ToListAsync();
        }

        // GET: api/Foods/5
        [HttpGet("{id}")]
        [Authorize(Roles = "RestaurantAdministrator, BrandAdministrator")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
          if (_context.Foods == null)
          {
              return NotFound();
          }
            var food = await _context.Foods.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }

        // PUT: api/Foods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<IActionResult> PutFood(int id, Food food)
        {
            if (id != food.Id)
            {
                return BadRequest();
            }

            _context.Entry(food).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(id))
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

        // POST: api/Foods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<ActionResult<Food>> PostFood(Food food)
        {
          if (_context.Foods == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Foods'  is null.");
          }
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        // DELETE: api/Foods/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food = await _context.Foods!.FindAsync(id);
            food!.StateId = 0;
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Foods/5
        //Restoran Yöneticisinin yemekleri açıp(active) kapatmasını(passive) sağlar.
        [HttpDelete("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public async Task<IActionResult> ChangeFoodState(int id)
        {
            var food = await _context.Foods!.FindAsync(id);
            food!.StateId = 2;
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Foods/UploadImage/5
        [HttpPost("UploadImage/{id}")]
        [Authorize(Roles = "BrandAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound("Yemek bulunamadı.");
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

            food.Photo = $"/images/{fileName}";
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla yüklendi.");
        }

        // PUT: api/Foods/EditImage/5
        [HttpPut("EditImage/{id}")]
        [Authorize(Roles = "BrandAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> EditImage(int id, IFormFile file)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound("Yemek bulunamadı.");
            }

            // Önce mevcut fotoğrafı silelim
            if (!string.IsNullOrEmpty(food.Photo))
            {
                var imagePath = Path.Combine("wwwroot", food.Photo.TrimStart('/'));
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

            food.Photo = $"/images/{fileName}";
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla yüklendi.");
        }


        // DELETE: api/Foods/DeleteImage/5
        [HttpDelete("DeleteImage/{id}")]
        [Authorize(Roles = "BrandAdministrator")] // İlgili rol için yetkilendirme yapılabilir.
        public async Task<IActionResult> DeleteImage(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound("Marka bulunamadı.");
            }

            if (string.IsNullOrEmpty(food.Photo))
            {
                return BadRequest("Markanın bir görseli bulunmuyor.");
            }

            var imagePath = Path.Combine("wwwroot", food.Photo.TrimStart('/'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            food.Photo = null;
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();

            return Ok("Resim başarıyla silindi.");
        }

        private bool FoodExists(int id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
