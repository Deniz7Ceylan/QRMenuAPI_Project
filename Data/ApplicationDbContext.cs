using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QRMenuAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<State>? States { get; set; }
    public DbSet<Company>? Companies { get; set; }
    public DbSet<Brand>? Brands { get; set; }
    public DbSet<Restaurant>? Restaurants { get; set; }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Food>? Foods { get; set; }
    public DbSet<BrandUser>? BrandUsers { get; set; }
    public DbSet<RestaurantUser>? RestaurantUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>().HasOne(au => au.State).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Company>().HasOne(co => co.State).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Brand>().HasOne(br => br.State).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Restaurant>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Category>().HasOne(ct => ct.State).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Food>().HasOne(f => f.State).WithMany().OnDelete(DeleteBehavior.NoAction);


        builder.Entity<BrandUser>().HasKey(bu => new { bu.UserId, bu.BrandId });
        builder.Entity<BrandUser>().HasOne(bu => bu.Brand).WithMany(b => b.BrandUsers).HasForeignKey(bu => bu.BrandId).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<BrandUser>().HasOne(bu => bu.ApplicationUser).WithMany(u => u.BrandUsers).HasForeignKey(bu => bu.UserId).OnDelete(DeleteBehavior.NoAction);

        builder.Entity<RestaurantUser>().HasKey(ru => new { ru.UserId, ru.RestaurantId });
        builder.Entity<RestaurantUser>().HasOne(ru => ru.Restaurant).WithMany(r => r.RestaurantUsers).HasForeignKey(ru => ru.RestaurantId).OnDelete(DeleteBehavior.NoAction);
        builder.Entity<RestaurantUser>().HasOne(ru => ru.ApplicationUser).WithMany(u => u.RestaurantUsers).HasForeignKey(ru => ru.UserId).OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(builder);
    }
}
