using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

public class UniversalODataController<TEntity> : ODataController where TEntity : class
{
    private readonly IDataService _dataService;

    public UniversalODataController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [EnableQuery(MaxExpansionDepth = 4, MaxTop = 1000)]
    public virtual IQueryable<TEntity> Get()
    {
        return _dataService.GetEntitySet<TEntity>();
    }

    [EnableQuery]
    public virtual async Task<IActionResult> Get([FromRoute] Guid key)
    {
        var entity = await _dataService.GetByIdAsync<TEntity>(key);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    public virtual async Task<IActionResult> Post([FromBody] TEntity entity)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _dataService.CreateAsync(entity);
        return Created(created);
    }

    public virtual async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] TEntity entity)
    {
        try
        {
            var updated = await _dataService.UpdateAsync(key, entity);
            return Updated(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    public virtual async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        try
        {
            await _dataService.DeleteAsync<TEntity>(key);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
