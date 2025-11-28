using MyData.EFCore.DbContexts;

public class DataService : IDataService
{
    private readonly AppDbContext _context;

    public DataService(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> GetEntitySet<T>() where T : class
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task<T?> GetByIdAsync<T>(Guid id) where T : class
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T> CreateAsync<T>(T entity) where T : class
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty?.PropertyType == typeof(Guid))
        {
            var currentId = (Guid)idProperty.GetValue(entity)!;
            if (currentId == Guid.Empty)
                idProperty.SetValue(entity, Guid.NewGuid());
        }

        var createdAtProperty = typeof(T).GetProperty("CreatedAt");
        if (createdAtProperty?.PropertyType == typeof(DateTime))
            createdAtProperty.SetValue(entity, DateTime.UtcNow);

        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync<T>(Guid id, T entity) where T : class
    {
        var existing = await _context.Set<T>().FindAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Entity with id {id} not found");

        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteAsync<T>(Guid id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Entity with id {id} not found");

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
