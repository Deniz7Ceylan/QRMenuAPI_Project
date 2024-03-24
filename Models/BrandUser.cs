using System.ComponentModel.DataAnnotations.Schema;

namespace QRMenuAPI.Models;

public class BrandUser
{
    public int BrandId { get; set; }
    [ForeignKey("BrandId")]
    public Brand? Brand { get; set; }

    public string? UserId { get; set; }
    [ForeignKey("UserId")]
    public ApplicationUser? ApplicationUser { get; set; }
}
