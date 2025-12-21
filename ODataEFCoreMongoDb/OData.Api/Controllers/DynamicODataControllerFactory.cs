// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using OData.Api.Models;
using OData.Api.Services;
using System.Reflection;
using System.Reflection.Emit;

namespace OData.Api.Controllers;

public class DynamicODataControllerFactory : IApplicationFeatureProvider<ControllerFeature>
{
  private readonly List<ODataEntityType> _odataEntityTypes;
  private readonly string _keyedServiceName;
  private static readonly AssemblyBuilder _assemblyBuilder;
  private static readonly ModuleBuilder _moduleBuilder;

  static DynamicODataControllerFactory()
  {
    var assemblyName = new AssemblyName("DynamicODataControllers");
    _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
    _moduleBuilder = _assemblyBuilder.DefineDynamicModule("MainModule");
  }

  public DynamicODataControllerFactory(string keyedServiceName, List<ODataEntityType> odataEntityTypes)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(keyedServiceName);

    _keyedServiceName = keyedServiceName;
    _odataEntityTypes = odataEntityTypes;
  }

  public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
  {
    foreach (var entityType in _odataEntityTypes)
    {
      var controllerType = CreateControllerType(entityType);
      feature.Controllers.Add(controllerType.GetTypeInfo());
    }
  }

  private Type CreateControllerType(ODataEntityType odataEntityType)
  {
    var controllerName = $"{odataEntityType.SetName}Controller";
    var baseType = typeof(UniversalODataController<>).MakeGenericType(odataEntityType.Type);

    var typeBuilder = _moduleBuilder.DefineType(
        controllerName,
        TypeAttributes.Public | TypeAttributes.Class,
        baseType
    );

    // Créer le constructeur avec IDataService en paramètre
    var constructorBuilder = typeBuilder.DefineConstructor(
        MethodAttributes.Public,
        CallingConventions.Standard,
        new[] { typeof(IDataService) }
    );

    // Définir le paramètre du constructeur
    var parameterBuilder = constructorBuilder.DefineParameter(
        1,  // Position du paramètre (1-based)
        ParameterAttributes.None,
        "dataService"
    );

    // Ajouter l'attribut [FromKeyedServices("<name>")] au paramètre
    var fromKeyedServicesAttribute = typeof(FromKeyedServicesAttribute);
    var attributeConstructor = fromKeyedServicesAttribute.GetConstructor(new[] { typeof(object) })!;
    var attributeBuilder = new CustomAttributeBuilder(
        attributeConstructor,
        new object[] { _keyedServiceName }
    );
    parameterBuilder.SetCustomAttribute(attributeBuilder);

    // Obtenir le constructeur de la classe de base
    var baseConstructor = baseType.GetConstructor(new[] { typeof(IDataService) })!;

    // Générer le code IL du constructeur
    var ilGenerator = constructorBuilder.GetILGenerator();

    // Appeler le constructeur de la classe de base : base(dataService)
    ilGenerator.Emit(OpCodes.Ldarg_0);        // Charger 'this'
    ilGenerator.Emit(OpCodes.Ldarg_1);        // Charger le paramètre 'dataService'
    ilGenerator.Emit(OpCodes.Call, baseConstructor); // Appeler base(dataService)
    ilGenerator.Emit(OpCodes.Ret);            // Return

    return typeBuilder.CreateType();
  }
}
