using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRMenuAPI.Models;

public class Restaurant
{
    public int Id { get; set; }

    [StringLength(200, MinimumLength = 2)]
    [Column(TypeName = "nvarchar(200)")]
    public string Name { get; set; } = "";

    public string City { get; set; } = "";

    public string District { get; set; } = "";

    [StringLength(200, MinimumLength = 5)]
    [Column(TypeName = "nvarchar(200)")]
    public string Address { get; set; } = "";

    [StringLength(5, MinimumLength = 5)]
    [Column(TypeName = "char(5)")]
    [DataType(DataType.PostalCode)]
    public string PostalCode { get; set; } = "";

    [Phone]
    [StringLength(30)]
    [Column(TypeName = "varchar(30)")]
    public string Phone { get; set; } = "";

    [EmailAddress]
    [StringLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string EMail { get; set; } = "";

    [Column(TypeName = "smalldatetime")]
    public DateTime RegisterDate { get; set; }

    //[StringLength(11, MinimumLength = 10)]
    //[Column(TypeName = "varchar(11)")]
    //public string TaxNumber { get; set; } = "";

    
    public int BrandId { get; set; }
    [ForeignKey("BrandId")]
    public Brand Brand { get; set; }

    public byte StateId { get; set; }
    [ForeignKey("StateId")]
    public State? State { get; set; }
    public ICollection<RestaurantUser>? RestaurantUsers { get; set; }
    public ICollection<Category>? Categories { get; set; }
}
