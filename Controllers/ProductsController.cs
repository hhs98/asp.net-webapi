using ProductApi.Models;
using ProductApi.Models.Dto;
//using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProductApi.Controllers

{
  [Authorize(Roles = "SuperAdmin,Admin,User")]
  [Route("api/ProductAPI")]
  [ApiController]
  public class ProductAPIController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public ProductAPIController(ApplicationDbContext context)
    {
      _context = context;
    }
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet]
    public ActionResult<IEnumerable<ProductDTO>> GetProducts()
    {
      return Ok(_context.Products);
    }

    [HttpGet("{id:int}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductDTO> GetProduct(int id)
    {
      if (id == 0)
      {
        return BadRequest();
      }


      var product = _context.Products.FirstOrDefault(u => u.ProductId == id);
      if (product == null)
      {
        return NotFound();
      }

      return Ok(product);

    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductDTO> CreateProduct([FromBody] ProductDTO productDTO)
    {
      //
      //check model state 
      //if (ModelState.IsValid)
      //{
      //    return BadRequest(ModelState);
      //}
      if (_context.Products.FirstOrDefault(u => u.Name.ToLower() == productDTO.Name.ToLower()) != null)
      {
        ModelState.AddModelError("CustomError", "Product already exists!");
        return BadRequest(ModelState);
      }

      if (productDTO == null)
      {
        return BadRequest(productDTO);
      }
      if (productDTO.ProductId > 0)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }

      Product model = new()
      {
        Name = productDTO.Name,
        Description = productDTO.Description,
        Price = productDTO.Price,
        Stock = productDTO.Stock
      };
      _context.Products.Add(model);
      _context.SaveChanges();

      return CreatedAtRoute("GetProduct", new { id = productDTO.ProductId }, productDTO);

    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpDelete("{id:int}", Name = "DeleteProduct")]

    public IActionResult DeleteProduct(int id)
    {
      if (id == 0)
      {
        return BadRequest();
      }
      var product = _context.Products.FirstOrDefault(u => u.ProductId == id);
      if (product == null)
      {
        return NotFound();
      }
      _context.Products.Remove(product);
      _context.SaveChanges();
      return NoContent();

    }


    [HttpPut("{id:int}", Name = "UpdateProduct")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public IActionResult UpdateProduct(int id, [FromBody] ProductDTO productDTO)
    {
      if (productDTO == null || id != productDTO.ProductId)
      {
        return BadRequest();
      }
      var product = _context.Products.FirstOrDefault(u => u.ProductId == id);
      // product.Name = productDTO.Name;
      // product.Description = productDTO.Description;
      // product.Price = productDTO.Price;
      // product.Stock = productDTO.Stock;

      Product model = new()
      {
        Name = productDTO.Name,
        Description = productDTO.Description,
        Price = productDTO.Price,
        Stock = productDTO.Stock
      };
      _context.Products.Update(model);
      _context.SaveChanges();

      return NoContent();
    }

    [HttpPatch("{id:int}", Name = "UpdatePartialProduct")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdatePartialProduct(int id, JsonPatchDocument<ProductDTO> patchDTO)
    {
      if (patchDTO == null || id == 0)
      {
        return BadRequest();
      }
      var product = _context.Products.FirstOrDefault(u => u.ProductId == id);

      ProductDTO productDTO = new()
      {
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock
      };
      if (product == null)
      {
        return BadRequest();
      }
      patchDTO.ApplyTo(productDTO, ModelState);
      Product model = new()
      {
        Name = productDTO.Name,
        Description = productDTO.Description,
        Price = productDTO.Price,
        Stock = productDTO.Stock
      };
      _context.Products.Update(model);
      _context.SaveChanges();
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      return NoContent();
    }
  }
}
