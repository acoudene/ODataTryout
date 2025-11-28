public interface IDataService
{
    IQueryable<T> GetEntitySet<T>() where T : class;
    Task<T?> GetByIdAsync<T>(Guid id) where T : class;
    Task<T> CreateAsync<T>(T entity) where T : class;
    Task<T> UpdateAsync<T>(Guid id, T entity) where T : class;
    Task DeleteAsync<T>(Guid id) where T : class;
}
