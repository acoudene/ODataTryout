// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Domain.EFCore.DbContexts;
using Domain.EFCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace MyApi.Controllers;

public class CategoriesController : ODataController
{
  private readonly DomainDbContext _context;

  public CategoriesController(DomainDbContext context)
  {
    _context = context;
  }

  [EnableQuery]
  public IQueryable<Category> Get()
  {
    //return _context.Categories.Include(c => c.Products);
    return _context.Categories;
  }

  [EnableQuery]
  public async Task<IActionResult> Get([FromRoute] Guid key)
  {
    //var category = await _context.Categories
    //    .Include(c => c.Products)
    //    .FirstOrDefaultAsync(c => c.Id == key);
    var category = await _context.Categories
        .FirstOrDefaultAsync(c => c.Id == key);
    if (category == null)
      return NotFound();

    return Ok(category);
  }
}
