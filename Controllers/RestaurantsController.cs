using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
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
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Restaurants
        [HttpGet]
        [Authorize(Roles = "BrandAdministrator, RestaurantAdministrator")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
          if (_context.Restaurants == null)
          {
              return NotFound();
          }
            return await _context.Restaurants.ToListAsync();
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        [Authorize(Roles = "BrandAdministrator, RestaurantAdministrator")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
          if (_context.Restaurants == null)
          {
              return NotFound();
          }
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        // PUT: api/Restaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<IActionResult> PutRestaurant(int id, Restaurant restaurant)
        {
            if (User.HasClaim("Restaurant", id.ToString()) || User.IsInRole("BrandAdministrator"))
            {
                _context.Entry(restaurant).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            if (id != restaurant.Id)
            {
                return BadRequest("Bu kimliğe sahip bir restoran yok.");
            }
            return Ok($"{restaurant.Name} güncellendi.");
        }

        // POST: api/Restaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<ActionResult<Restaurant>> PostRestaurant(Restaurant restaurant, string userName, string password)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            Claim claim;

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            applicationUser.Name = restaurant.Name;
            applicationUser.PhoneNumber = restaurant.Phone;
            applicationUser.CompanyId = restaurant.BrandId;
            applicationUser.StateId = 1;
            applicationUser.UserName = userName;

            var result = _userManager.CreateAsync(applicationUser, password).Result;
            claim = new Claim("Restaurant", restaurant.Id.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();

            _userManager.AddToRoleAsync(applicationUser, "RestaurantAdministrator").Wait();

            return Ok($"{restaurant.Name} güncellendi.");
        }

        // DELETE: api/Restaurants/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            if (User.HasClaim("BrandId", id.ToString()) || User.IsInRole("BrandAdministrator"))
            {
                var restaurant = _context.Restaurants!.FindAsync(id).Result;
                restaurant!.StateId = 0;
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
                await _context.SaveChangesAsync();

                return Content($"{restaurant.Name} alt katmanları silindi.");
            }
            return NotFound();
        }

        // DELETE: api/Restaurants/5
        [HttpDelete("Restaurant/{id}")]
        [Authorize(Roles = "BrandAdministrator")]
        public async Task<IActionResult> ChangeRestaurantState(int id)
        {
            if (User.HasClaim("BrandId", id.ToString()) || User.IsInRole("BrandAdministrator"))
            {
                var restaurant = _context.Restaurants!.FindAsync(id).Result;
                restaurant!.StateId = 2;
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
                await _context.SaveChangesAsync();

                return Content($"{restaurant.Name} alt katmanları kapatıldı/pasifleştirildi.");
            }
            return NotFound();
        }

        private bool RestaurantExists(int id)
        {
            return (_context.Restaurants?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
