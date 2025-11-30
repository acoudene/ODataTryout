// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.EntityFrameworkCore;
using OData.Api.Models;

namespace Microsoft.OData.ModelBuilder;

public static class ODataConventionModelBuilderExtensions
{
  /// <summary>
  /// Registers all entity sets found in the specified <see cref="DbContext"/> type with the OData model builder and returns the corresponding entity types.
  /// </summary>
  /// <typeparam name="T">
  /// The type of <see cref="DbContext"/> containing <see cref="DbSet{TEntity}"/> properties to be registered as entity sets.
  /// </typeparam>
  /// <param name="modelBuilder">
  /// The <see cref="ODataConventionModelBuilder"/> instance used to configure the OData entity sets.
  /// </param>
  /// <returns>
  /// A list of <see cref="ODataEntityType"/> objects representing the entity types that were registered as entity sets.
  /// </returns>
  public static List<ODataEntityType> AutoRegisterEntities<T>(this ODataConventionModelBuilder modelBuilder)
    where T : DbContext
  {
    return modelBuilder.AutoRegisterEntities(typeof(T));
  }

  /// <summary>
  /// Registers all entity sets found in the specified <see cref="DbContext"/> type with the OData model builder and returns the corresponding entity types.
  /// </summary>
  /// <remarks>
  /// This method scans the provided <see cref="DbContext"/> type for properties of type <see cref="DbSet{TEntity}"/> and registers each entity type <c>TEntity</c> as an entity set in the OData model builder.
  /// Only public <see cref="DbSet{TEntity}"/> properties are considered. This can simplify OData model configuration by automatically registering all entities defined in the <see cref="DbContext"/>.
  /// </remarks>
  /// <param name="modelBuilder">
  /// The <see cref="ODataConventionModelBuilder"/> instance used to configure the OData entity sets.
  /// </param>
  /// <param name="dbContextType">
  /// The <see cref="Type"/> of the <see cref="DbContext"/> containing <see cref="DbSet{TEntity}"/> properties to be registered as entity sets. Must not be null.
  /// </param>
  /// <returns>
  /// A list of <see cref="ODataEntityType"/> objects representing the entity types that were registered as entity sets.
  /// </returns>
  public static List<ODataEntityType> AutoRegisterEntities(this ODataConventionModelBuilder modelBuilder, Type dbContextType)
  {
    var odataentityTypes = new List<ODataEntityType>();
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
      odataentityTypes.Add(new ODataEntityType { Type = entityType, SetName = property.Name });
    }

    return odataentityTypes;
  }
}
