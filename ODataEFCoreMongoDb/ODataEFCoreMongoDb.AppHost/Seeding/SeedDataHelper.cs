// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ODataEFCoreMongoDb.AppHost.Seeding;

/// <summary>
/// Generate Data
/// </summary>
internal static class SeedDataHelper
{
  /// <summary>
  /// Import data from a json file to a collection
  /// </summary>
  /// <returns></returns>
  public static TCollection[] DeserializeForCollection<TCollection>(string fileName) where TCollection : class /*, IEntity*/
  {
    if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

    var jsonString = File.ReadAllText(fileName);
    return BsonSerializer.Deserialize<TCollection[]>(jsonString)!;
  }

  /// <summary>
  /// Imports data into a specified MongoDB collection from a file.
  /// </summary>
  /// <remarks>This method establishes a connection to the MongoDB instance using the provided connection string
  /// and delegates the import operation to an overloaded method that accepts a <see cref="MongoClient"/>
  /// instance.</remarks>
  /// <typeparam name="TCollection">The type of the collection's documents. Must be a reference type.</typeparam>
  /// <param name="connectionString">The connection string used to connect to the MongoDB instance. Cannot be null, empty, or whitespace.</param>
  /// <param name="databaseName">The name of the MongoDB database where the collection resides. Cannot be null, empty, or whitespace.</param>
  /// <param name="collectionName">The name of the collection into which the data will be imported. Cannot be null, empty, or whitespace.</param>
  /// <param name="fileName">The path to the file containing the data to be imported. Cannot be null, empty, or whitespace.</param>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="connectionString"/>, <paramref name="databaseName"/>, <paramref name="collectionName"/>,
  /// or <paramref name="fileName"/> is null, empty, or consists only of whitespace.</exception>
  public static void ImportDataByCollection<TCollection>(string connectionString,
                                                         string databaseName,
                                                         string collectionName,
                                                         string fileName)
      where TCollection : class /*, IEntity*/
  {
    if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
    if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentNullException(nameof(databaseName));
    if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));
    if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

    var mongoClient = new MongoClient(connectionString);
    ImportDataByCollection<TCollection>(mongoClient, databaseName, collectionName, fileName);
  }

  /// <summary>
  /// Imports data into a specified MongoDB collection from a file.
  /// </summary>
  /// <remarks>This method connects to the specified MongoDB database and delegates the import operation to an
  /// overloaded method that performs the actual data import. Ensure that the file format matches the expected structure
  /// for the collection.</remarks>
  /// <typeparam name="TCollection">The type of the collection's documents. Must be a reference type.</typeparam>
  /// <param name="mongoClient">The <see cref="IMongoClient"/> instance used to connect to the MongoDB server. Cannot be <see langword="null"/>.</param>
  /// <param name="databaseName">The name of the MongoDB database. Cannot be <see langword="null"/>, empty, or whitespace.</param>
  /// <param name="collectionName">The name of the MongoDB collection where the data will be imported. Cannot be <see langword="null"/>, empty, or
  /// whitespace.</param>
  /// <param name="fileName">The path to the file containing the data to be imported. Cannot be <see langword="null"/>, empty, or whitespace.</param>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="mongoClient"/>, <paramref name="databaseName"/>, <paramref name="collectionName"/>, or
  /// <paramref name="fileName"/> is <see langword="null"/> or invalid.</exception>
  public static void ImportDataByCollection<TCollection>(IMongoClient mongoClient,
                                                         string databaseName,
                                                         string collectionName,
                                                         string fileName)
    where TCollection : class /*, IEntity*/
  {
    if (mongoClient is null) throw new ArgumentNullException(nameof(mongoClient));
    if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentNullException(nameof(databaseName));
    if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));
    if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

    var mongoDatabase = mongoClient.GetDatabase(databaseName);
    ImportDataByCollection<TCollection>(mongoDatabase, collectionName, fileName);
  }

  /// <summary>
  /// Imports data into a MongoDB collection from a specified file.
  /// </summary>
  /// <remarks>This method retrieves the specified collection from the provided MongoDB database and imports
  /// data into it from the specified file. The file format and structure must be compatible with the expected document
  /// type.</remarks>
  /// <typeparam name="TCollection">The type of the documents in the MongoDB collection. Must be a reference type.</typeparam>
  /// <param name="mongoDatabase">The MongoDB database instance where the collection resides. Cannot be <see langword="null"/>.</param>
  /// <param name="collectionName">The name of the MongoDB collection into which the data will be imported. Cannot be <see langword="null"/> or
  /// whitespace.</param>
  /// <param name="fileName">The path to the file containing the data to be imported. Cannot be <see langword="null"/> or whitespace.</param>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="mongoDatabase"/>, <paramref name="collectionName"/>, or <paramref name="fileName"/> is
  /// <see langword="null"/> or whitespace.</exception>
  public static void ImportDataByCollection<TCollection>(IMongoDatabase mongoDatabase,
                                                         string collectionName,
                                                         string fileName)
    where TCollection : class /*, IEntity*/
  {
    if (mongoDatabase is null) throw new ArgumentNullException(nameof(mongoDatabase));
    if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));
    if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

    var mongoCollection = mongoDatabase.GetCollection<TCollection>(collectionName);
    ImportDataByCollection(mongoCollection, fileName);
  }

  /// <summary>
  /// Imports data into the specified MongoDB collection from a file.
  /// </summary>
  /// <remarks>The method deserializes the data from the specified file and inserts it into the provided MongoDB
  /// collection. Ensure that the file format matches the expected structure of the <typeparamref name="TCollection"/>
  /// type.</remarks>
  /// <typeparam name="TCollection">The type of the documents in the MongoDB collection. Must be a reference type.</typeparam>
  /// <param name="mongoCollection">The MongoDB collection where the data will be imported. Cannot be <see langword="null"/>.</param>
  /// <param name="fileName">The path to the file containing the data to import. Cannot be <see langword="null"/> or whitespace.</param>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="mongoCollection"/> is <see langword="null"/> or if <paramref name="fileName"/> is <see
  /// langword="null"/> or whitespace.</exception>
  public static void ImportDataByCollection<TCollection>(IMongoCollection<TCollection> mongoCollection,
                                                         string fileName)
    where TCollection : class /*, IEntity*/
  {
    if (mongoCollection is null) throw new ArgumentNullException(nameof(mongoCollection));
    if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

    var data = DeserializeForCollection<TCollection>(fileName);
    mongoCollection.InsertMany(data);
  }

  
}
