// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Domain.EFCore.DbContexts;
using Domain.EFCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MyApi.Controllers;

public class OrdersController : ODataController
{
  private readonly DomainDbContext _context;

  public OrdersController(DomainDbContext context)
  {
    _context = context;
  }

  [EnableQuery]
  public IQueryable<Order> Get()
  {
    return _context.Orders;
  }

  public async Task<IActionResult> Post([FromBody] Order order)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    order.Id = Guid.NewGuid();
    order.OrderDate = DateTime.UtcNow;

    _context.Orders.Add(order);
    await _context.SaveChangesAsync();

    return Created(order);
  }
}