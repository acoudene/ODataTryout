using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MyApi.DbContexts;
using MyApi.Entities;

namespace MyApi.Controllers;

public class OrdersController : ODataController
{
  private readonly AppDbContext _context;

  public OrdersController(AppDbContext context)
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