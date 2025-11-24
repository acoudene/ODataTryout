using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using MyApi.DbContexts;
using MyApi.Entities;

namespace MyApi.Controllers;

public class ProductsController : ODataController
{
  private readonly AppDbContext _context;

  public ProductsController(AppDbContext context)
  {
    _context = context;
  }

  [EnableQuery(MaxExpansionDepth = 3)]
  public IQueryable<Product> Get()
  {
    return _context.Products.Include(p => p.Category);
  }

  [EnableQuery]
  public async Task<IActionResult> Get([FromRoute] Guid key)
  {
    var product = await _context.Products
        .Include(p => p.Category)
        .FirstOrDefaultAsync(p => p.Id == key);

    if (product == null)
      return NotFound();

    return Ok(product);
  }

  public async Task<IActionResult> Post([FromBody] Product product)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    product.Id = Guid.NewGuid();
    product.CreatedAt = DateTime.UtcNow;

    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    return Created(product);
  }

  public async Task<IActionResult> Patch([FromRoute] Guid key, [FromBody] Product updatedProduct)
  {
    var product = await _context.Products.FindAsync(key);
    if (product == null)
      return NotFound();

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    product.Stock = updatedProduct.Stock;
    product.CategoryId = updatedProduct.CategoryId;

    await _context.SaveChangesAsync();
    return Updated(product);
  }

  public async Task<IActionResult> Delete([FromRoute] Guid key)
  {
    var product = await _context.Products.FindAsync(key);
    if (product == null)
      return NotFound();

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();

    return NoContent();
  }
}
