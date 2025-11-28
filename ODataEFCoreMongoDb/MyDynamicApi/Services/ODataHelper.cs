using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

namespace MyDynamicApi.Services;

public static class ODataHelper
{
    public static List<Type> AutoRegisterEntities(ODataConventionModelBuilder modelBuilder, Type dbContextType)
    {
        var entityTypes = new List<Type>();
        var dbSetProperties = dbContextType.GetProperties()
            .Where(p => p.PropertyType.IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        foreach (var property in dbSetProperties)
        {
            var entityType = property.PropertyType.GetGenericArguments()[0];
            var entitySetMethod = typeof(ODataConventionModelBuilder)
                .GetMethod("EntitySet")!
                .MakeGenericMethod(entityType);

            entitySetMethod.Invoke(modelBuilder, new[] { property.Name });
            entityTypes.Add(entityType);

            Console.WriteLine($"✓ Auto-registered: {property.Name} ({entityType.Name})");
        }

        return entityTypes;
    }
}
