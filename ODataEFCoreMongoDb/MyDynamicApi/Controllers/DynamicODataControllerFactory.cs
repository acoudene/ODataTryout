using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using System.Reflection.Emit;

public class DynamicODataControllerFactory : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly List<Type> _entityTypes;
    private static readonly AssemblyBuilder _assemblyBuilder;
    private static readonly ModuleBuilder _moduleBuilder;

    static DynamicODataControllerFactory()
    {
        var assemblyName = new AssemblyName("DynamicODataControllers");
        _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        _moduleBuilder = _assemblyBuilder.DefineDynamicModule("MainModule");
    }

    public DynamicODataControllerFactory(List<Type> entityTypes)
    {
        _entityTypes = entityTypes;
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        foreach (var entityType in _entityTypes)
        {
            var controllerType = CreateControllerType(entityType);
            feature.Controllers.Add(controllerType.GetTypeInfo());
        }
    }

    private Type CreateControllerType(Type entityType)
    {
        var controllerName = $"{entityType.Name}sController";
        var baseType = typeof(UniversalODataController<>).MakeGenericType(entityType);

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

        // Obtenir le constructeur de la classe de base
        var baseConstructor = baseType.GetConstructor(new[] { typeof(IDataService) })!;

        // Générer le code IL du constructeur
        var ilGenerator = constructorBuilder.GetILGenerator();

        // Appeler le constructeur de la classe de base : base(dataService)
        ilGenerator.Emit(OpCodes.Ldarg_0);        // Charger 'this'
        ilGenerator.Emit(OpCodes.Ldarg_1);        // Charger le paramètre 'dataService'
        ilGenerator.Emit(OpCodes.Call, baseConstructor); // Appeler base(dataService)
        ilGenerator.Emit(OpCodes.Ret);            // Return

        return typeBuilder.CreateType()!;
    }
}
