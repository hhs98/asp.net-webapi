using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace ProductApi.Controllers
{
  [Authorize(Roles = "SuperAdmin,Admin,User")]
  [ApiController]
  [Route("api/[controller]")]
  public class ProductController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public ProductController(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
      var products = await _context.Products.ToListAsync();
      return Ok(products);
    }

    [HttpGet("{id:int}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDTO>> GetProduct(int id)
    {
      var product = await _context.Products.FindAsync(id);

      if (product == null)
      {
        return NotFound();
      }

      return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] ProductDTO product)
    {
      if (product == null)
      {
        return BadRequest();
      }

      await _context.Products.AddAsync(product);
      await _context.SaveChangesAsync();

      return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO product)
    {
      if (product == null || id != product.Id)
      {
        return BadRequest();
      }

      var existingProduct = await _context.Products.FindAsync(id);
      if (existingProduct == null)
      {
        return NotFound();
      }

      existingProduct.Name = product.Name;
      existingProduct.Description = product.Description;
      existingProduct.Price = product.Price;
      existingProduct.InStock = product.InStock;

      await _context.SaveChangesAsync();

      return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
      var product = await _context.Products.FindAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      _context.Products.Remove(product);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialProduct")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialProduct(int id, JsonPatchDocument<ProductDTO> patchDTO)
    {
      if (patchDTO == null || id == 0)
      {
        return BadRequest();
      }
      var product = await _context.Products.FindAsync(id);
      if (product == null)
      {
        return BadRequest();
      }
      patchDTO.ApplyTo(product, ModelState);
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      await _context.SaveChangesAsync();
      return NoContent();
    }
  }
}
