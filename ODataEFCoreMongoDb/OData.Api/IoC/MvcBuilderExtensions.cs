// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using OData.Api.Controllers;

namespace OData.Api.IoC;

public static class MvcBuilderExtensions
{
  public static IMvcBuilder AddDefaultODataDynamicControllerFeature(this IMvcBuilder builder,
    string keyedServiceName,
    List<Type> entityTypes,
    IEdmModel edmModel,
    string routePrefix)
  {
    var controllerFactory = new DynamicODataControllerFactory(keyedServiceName, entityTypes);

    return builder
    .ConfigureApplicationPartManager(apm =>
    {
      apm.FeatureProviders.Add(controllerFactory);
    })
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(1000)
        .AddRouteComponents(routePrefix, edmModel));
  }
}
