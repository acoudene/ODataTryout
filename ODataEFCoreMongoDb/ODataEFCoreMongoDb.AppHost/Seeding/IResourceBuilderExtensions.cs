// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using MongoDB.Bson;
using MongoDB.Driver;

namespace ODataEFCoreMongoDb.AppHost.Seeding;

public static class IResourceBuilderExtensions
{
  public static IResourceBuilder<MongoDBDatabaseResource> SeedOnResourceReady(
    this IResourceBuilder<MongoDBDatabaseResource> builder,
    List<string> collectionNames)
    => builder.OnResourceReady(async (mongoDBDatabaseResource, cnxStringEvent, cancellationToken) =>
        await mongoDBDatabaseResource.SeedAsync(collectionNames, cancellationToken));

  public static async Task SeedAsync(
    this MongoDBDatabaseResource mongoDBDatabaseResource,
    List<string> collectionNames,
    CancellationToken cancellationToken = default)
  {
    string? connectionString = await mongoDBDatabaseResource.ConnectionStringExpression
      .GetValueAsync(cancellationToken);

    if (string.IsNullOrWhiteSpace(connectionString))
      return;

    var mongoClient = new MongoClient(connectionString);
    string databaseName = mongoDBDatabaseResource.DatabaseName;

    collectionNames.ForEach(collectionName =>
    SeedDataHelper.ImportDataByCollection<BsonDocument>(
    mongoClient,
    databaseName,
    collectionName,
    $"Data/{databaseName}.{collectionName}.json"));
  }
}
