using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QRMenuAPI.Models;

public class Food
{
    public int Id { get; set; }

    [StringLength(100, MinimumLength = 2)]
    [Column(TypeName = "nvarchar(100)")]
    public string Name { get; set; } = "";

    public string? Photo { get; set; }

    //Besin Değerleri
    [Range(0, float.MaxValue)]
    public float? EnergyKcal { get; set; } // Enerji (Kcal)
    [Range(0, float.MaxValue)]
    public float? Protein { get; set; }    // Protein (g)
    [Range(0, float.MaxValue)]
    public float? Carbohydrate { get; set; } // Karbonhidrat (g)
    [Range(0, float.MaxValue)]
    public float? Fat { get; set; }          // Yağ (g)
    [Range(0, float.MaxValue)]
    public float? SaturatedFat { get; set; } // Doymuş yağ (g)
    [Range(0, float.MaxValue)]
    public float? Sodium { get; set; }       // Sodyum (mg)

    [StringLength(200)]
    [Column(TypeName = "nvarchar(100)")]
    public string? Ingredients { get; set; } //Ürün İçeriği

    [Range(0, float.MaxValue)]
    public float Price { get; set; }

    [StringLength(200)]
    [Column(TypeName = "nvarchar(200)")]
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public byte StateId { get; set; }
    [ForeignKey("StateId")]
    public State? State { get; set; }
}
