namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Macabresoft.Macabre2D.Framework;
using Mono.Cecil;
using Unity;

/// <summary>
/// Interface for a service which loads types from assemblies.
/// </summary>
public interface IAssemblyService {
    /// <summary>
    /// Creates an object by type with the provided constructor parameters.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="constructorParameters">The constructor parameters.</param>
    /// <returns>The constructed object.</returns>
    object CreateObjectFromType(Type type, params object[] constructorParameters);

    /// <summary>
    /// Loads assemblies from files in the specified directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <returns>A task.</returns>
    void LoadAssemblies(string directory);

    /// <summary>
    /// Loads the first type of the specified type as a generic type given the type arguments.
    /// </summary>
    /// <param name="genericType">The base type.</param>
    /// <param name="typeArguments">The type arguments for the generic type.</param>
    /// <returns>The generic type with the specified type arguments, if it exists as a discrete class.</returns>
    Type LoadFirstGenericType(Type genericType, params Type[] typeArguments);

    /// <summary>
    /// Loads the first type of the specified type with the specified base type in the current application domain.
    /// </summary>
    /// <param name="baseType">The base type.</param>
    /// <remarks>This might not be super useful given <see cref="LoadFirstGenericType" />.</remarks>
    /// <returns>The type.</returns>
    Type LoadFirstType(Type baseType);

    /// <summary>
    /// Loads all types that implement the specified base type in the current application domain.
    /// </summary>
    /// <param name="baseType"></param>
    /// <returns>An enumerable of types.</returns>
    IEnumerable<Type> LoadTypes(Type baseType);
}

/// <summary>
/// A service which loads types from assemblies.
/// </summary>
public sealed class AssemblyService : IAssemblyService {
    private readonly IUnityContainer _container;
    private bool _hasLoaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyService" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    public AssemblyService(IUnityContainer container) {
        this._container = container;
    }

    /// <inheritdoc />
    public object CreateObjectFromType(Type type, params object[] constructorParameters) {
        return this._container.Resolve(type, new GenericParameterOverride(constructorParameters));
    }


    /// <inheritdoc />
    public void LoadAssemblies(string directory) {
        if (!this._hasLoaded && Directory.Exists(directory)) {
            try {
                var assemblyPaths = Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories);
                foreach (var assemblyPath in assemblyPaths) {
                    try {
                        if (assemblyPath.HasObjectsOfType<IEntity>() || assemblyPath.HasObjectsOfType<IUpdateableSystem>()) {
                            Assembly.LoadFile(assemblyPath);
                        }
                    }
                    catch (FileLoadException) {
                    }
                    catch (BadImageFormatException) {
                    }
                }
            }
            finally {
                this._hasLoaded = true;
            }
        }
    }

    /// <inheritdoc />
    public Type LoadFirstGenericType(Type genericType, params Type[] typeArguments) {
        return this.LoadFirstType(genericType.MakeGenericType(typeArguments));
    }

    /// <inheritdoc />
    public Type LoadFirstType(Type baseType) {
        Type resultType = null;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var filter = baseType.IsGenericTypeDefinition ? type => this.CheckIfTypeMatchGeneric(baseType, type) : new Func<Type, bool>(type => this.CheckIfTypeMatch(baseType, type));
        foreach (var assembly in assemblies) {
            try {
                resultType = assembly.GetTypes().Where(filter).FirstOrDefault();
            }
            catch (FileLoadException) {
            }
            catch (BadImageFormatException) {
            }
            catch (ReflectionTypeLoadException) {
            }

            if (resultType != null) {
                break;
            }
        }

        return resultType;
    }

    /// <inheritdoc />
    public IEnumerable<Type> LoadTypes(Type baseType) {
        var types = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var filter = baseType.IsGenericTypeDefinition ? type => this.CheckIfTypeMatchGeneric(baseType, type) : new Func<Type, bool>(type => this.CheckIfTypeMatch(baseType, type));
        foreach (var assembly in assemblies) {
            try {
                types.AddRange(assembly.GetTypes().Where(filter).ToList());
            }
            catch (FileLoadException) {
            }
            catch (BadImageFormatException) {
            }
            catch (ReflectionTypeLoadException) {
            }
        }

        return types;
    }

    private bool CheckIfTypeMatch(Type baseType, Type testingType) {
        return baseType != testingType && !testingType.IsAbstract && testingType.IsPublic && baseType.IsAssignableFrom(testingType);
    }

    private bool CheckIfTypeMatchGeneric(Type baseType, Type testingType) {
        return baseType != testingType && !testingType.IsAbstract && testingType.IsPublic && testingType.BaseType?.IsGenericType == true && testingType.BaseType.GetGenericTypeDefinition() == baseType;
    }
}

internal static class AssemblyExtensions {
    internal static bool HasObjectsOfType<T>(this string assemblyPath) {
        var definition = AssemblyDefinition.ReadAssembly(assemblyPath);
        var result = false;
        var type = typeof(T);

        if (definition != null) {
            result = definition.MainModule.Types.Any(x => x.BaseType != null && x.BaseType.FullName == type.FullName && x.BaseType.Namespace == type.Namespace);
        }

        return result;
    }
}