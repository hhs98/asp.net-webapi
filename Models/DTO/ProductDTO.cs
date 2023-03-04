using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models.Dto
{
  public class ProductDTO
  {
    public int ProductId { get; set; }
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required]
    public double Price { get; set; }
    public int Stock { get; set; }
  }
}
