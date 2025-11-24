using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using MyApi.DbContexts;
using MyApi.Entities;

namespace MyApi.Controllers;

public class CategoriesController : ODataController
{
  private readonly AppDbContext _context;

  public CategoriesController(AppDbContext context)
  {
    _context = context;
  }

  [EnableQuery]
  public IQueryable<Category> Get()
  {
    return _context.Categories.Include(c => c.Products);
  }

  [EnableQuery]
  public async Task<IActionResult> Get([FromRoute] Guid key)
  {
    var category = await _context.Categories
        .Include(c => c.Products)
        .FirstOrDefaultAsync(c => c.Id == key);

    if (category == null)
      return NotFound();

    return Ok(category);
  }
}
