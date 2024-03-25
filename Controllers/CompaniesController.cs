using System;
using System.Collections.Generic;
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
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Companies
        [HttpGet]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
          if (_context.Companies == null)
          {
              return NotFound();
          }
            return await _context.Companies.ToListAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
          if (_context.Companies == null)
          {
              return NotFound();
          }
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        // PUT: api/Companies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

        // POST: api/Companies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
          if (_context.Companies == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
          }
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (User.HasClaim("CompanyId", id.ToString()) || User.IsInRole("CompanyAdministrator"))
            {
                var company = _context.Companies!.FindAsync(id).Result;
                company!.StateId = 0;
                _context.Companies.Update(company);

                var brands = _context.Brands.Where(r => r.CompanyId == id);
                foreach (Brand brand in brands)
                {
                    brand.StateId = 0;
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
                }

                await _context.SaveChangesAsync();

                return Content("Şirketin alt katmanları silindi.");
            }
            return NotFound();
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
