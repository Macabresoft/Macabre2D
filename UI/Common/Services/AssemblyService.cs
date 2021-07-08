namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Macabresoft.Macabre2D.Framework;
    using Mono.Cecil;

    /// <summary>
    /// Interface for a service which loads types from assemblies.
    /// </summary>
    public interface IAssemblyService {
        /// <summary>
        /// Loads assemblies from files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>A task.</returns>
        void LoadAssemblies(string directory);

        /// <summary>
        /// Loads the first type of the specified type with the specified base type in the current application domain.
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        Type LoadFirstType(Type baseType);

        /// <summary>
        /// Loads all types that implement the specified base type in the current application domain.
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns>A list of types.</returns>
        IList<Type> LoadTypes(Type baseType);
    }

    /// <summary>
    /// A service which loads types from assemblies.
    /// </summary>
    public sealed class AssemblyService : IAssemblyService {
        private bool _hasLoaded;

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
        public IList<Type> LoadTypes(Type baseType) {
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
}