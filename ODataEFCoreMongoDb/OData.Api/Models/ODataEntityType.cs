namespace OData.Api.Models;

public record ODataEntityType
{
  public required Type Type { get; set; }
  public required string SetName { get; set; }
}
