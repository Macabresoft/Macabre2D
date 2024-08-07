namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;

/// <summary>
/// Interface for a service which loads types from assemblies.
/// </summary>
public interface IAssemblyService {

    /// <summary>
    /// Creates a content file object with the given parent and metadata.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="metadata">The metadata.</param>
    /// <returns>The content file.</returns>
    ContentFile CreateContentFileObject(IContentDirectory parent, ContentMetadata metadata);

    /// <summary>
    /// Creates an object by type with the provided constructor parameters.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="constructorParameters">The constructor parameters.</param>
    /// <returns>The constructed object.</returns>
    object CreateObjectFromType(Type type, params object[] constructorParameters);

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
    private readonly IReadOnlyCollection<Assembly> _assemblies;
    private readonly IUnityContainer _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyService" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    [InjectionConstructor]
    public AssemblyService(IUnityContainer container) : this(container, true) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyService" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="includeEditorAssembly">A value indicating whether to include the editor assembly.</param>
    [InjectionConstructor]
    public AssemblyService(IUnityContainer container, bool includeEditorAssembly) {
        this._container = container;

        var assemblies = new List<Assembly>();

        if (includeEditorAssembly) {
            assemblies.Add(Assembly.Load("Macabre2D"));
        }

        assemblies.Add(Assembly.Load("Macabre2D.Framework"));
        assemblies.Add(Assembly.Load("Macabre2D.Project.Gameplay"));
        assemblies.Add(Assembly.Load("Macabre2D.Project.Common"));
        assemblies.Add(Assembly.Load("Macabre2D.UI.Common"));
        this._assemblies = assemblies;
    }

    /// <inheritdoc />
    public ContentFile CreateContentFileObject(IContentDirectory parent, ContentMetadata metadata) {
        ContentFile contentFile = null;
        var contentFileType = this.LoadFirstGenericType(typeof(ContentFile<>), metadata.Asset.GetType());
        if (contentFileType != null) {
            contentFile = this.CreateObjectFromType(contentFileType, parent, metadata) as ContentFile;
        }

        return contentFile ?? new ContentFile(parent, metadata);
    }

    /// <inheritdoc />
    public object CreateObjectFromType(Type type, params object[] constructorParameters) => this._container.Resolve(type, new GenericParameterOverride(constructorParameters));

    /// <inheritdoc />
    public Type LoadFirstGenericType(Type genericType, params Type[] typeArguments) => this.LoadFirstType(genericType.MakeGenericType(typeArguments));

    /// <inheritdoc />
    public Type LoadFirstType(Type baseType) {
        Type resultType = null;
        var filter = baseType.IsGenericTypeDefinition ? type => CheckIfTypeMatchGeneric(baseType, type) : new Func<Type, bool>(type => CheckIfTypeMatch(baseType, type));

        foreach (var assembly in this._assemblies) {
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
        var filter = baseType.IsGenericTypeDefinition ? type => CheckIfTypeMatchGeneric(baseType, type) : new Func<Type, bool>(type => CheckIfTypeMatch(baseType, type));

        foreach (var assembly in this._assemblies) {
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

    private static bool CheckIfTypeMatch(Type baseType, Type testingType) => baseType != testingType && !testingType.IsAbstract && testingType.IsPublic && baseType.IsAssignableFrom(testingType);

    private static bool CheckIfTypeMatchGeneric(Type baseType, Type testingType) => baseType != testingType && !testingType.IsAbstract && testingType.IsPublic && testingType.BaseType?.IsGenericType == true && testingType.BaseType.GetGenericTypeDefinition() == baseType;
}