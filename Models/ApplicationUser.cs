using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRMenuAPI.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100, MinimumLength = 2)]
    [Column(TypeName = "nvarchar(100)")]
    public override string UserName { get => base.UserName; set => base.UserName = value; }

    /*[Required]
    [StringLength(100)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";*/

    [StringLength(100, MinimumLength = 2)]
    [Column(TypeName = "nvarchar(100)")]
    public string? Name { get; set; } = "";

    [StringLength(100, MinimumLength = 2)]
    [Column(TypeName = "nvarchar(100)")]
    public string? SurName { get; set; } = "";

    [EmailAddress]
    [StringLength(100)]
    [Column(TypeName = "varchar(100)")]
    public override string Email { get => base.Email; set => base.Email = value; }

    public override string? PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }

    public DateTime RegisterDate { get; set; }

    public int CompanyId { get; set; }
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }
    public byte StateId { get; set; }
    [ForeignKey("StateId")]
    public State? State { get; set; }

    public virtual ICollection<BrandUser>? BrandUsers { get; set; }
    public virtual ICollection<RestaurantUser>? RestaurantUsers { get; set; }
}

