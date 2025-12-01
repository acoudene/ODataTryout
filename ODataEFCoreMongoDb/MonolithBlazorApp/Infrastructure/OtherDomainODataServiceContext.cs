using Microsoft.OData.Client;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using OtherDomain.EFCore.Entities;

namespace MonolithBlazorApp.Infrastructure;

public partial class OtherDomainODataServiceContext : DataServiceContext
{
  public OtherDomainODataServiceContext(Uri serviceRoot) : base(serviceRoot)
  {
    //Add this to override the default HttpWebRequest for making requests in OData Client. 
    //HttpRequestTransportMode = HttpRequestTransportMode.HttpClient;

    SuperHeroes = base.CreateQuery<SuperHero>("SuperHeroes");
    Format.LoadServiceModel = () => GetEdmModel();
    Format.UseJson();
  }

  private IEdmModel GetEdmModel()
  {
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<SuperHero>("SuperHeroes")
      .EntityType
      .HasKey(p => p.Id)
      .Count()
      .Select()
      .Page(null, 100)
      .Expand()
      .Filter();
    return builder.GetEdmModel();
  }

  public DataServiceQuery<SuperHero> SuperHeroes { get; }
}
