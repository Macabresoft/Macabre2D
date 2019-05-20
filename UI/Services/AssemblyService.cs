namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.ServiceInterfaces;
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public sealed class AssemblyService : IAssemblyService {
        private bool _hasLoaded = false;

        public async Task LoadAssemblies(string path) {
            if (!this._hasLoaded && Directory.Exists(path)) {
                try {
                    await Task.Run(() => {
                        var assemblyPaths = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
                        foreach (var assemblyPath in assemblyPaths) {
                            try {
                                if (assemblyPath.HasObjectsOfType<BaseComponent>() || assemblyPath.HasObjectsOfType<BaseModule>()) {
                                    Assembly.LoadFile(assemblyPath);
                                }
                            }
                            catch (FileLoadException) {
                            }
                            catch (BadImageFormatException) {
                            }
                        }
                    });
                }
                finally {
                    this._hasLoaded = true;
                }
            }
        }

        public async Task<IList<Type>> LoadTypes(Type baseType) {
            return await Task.Run(() => {
                var types = new List<Type>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var filter = baseType.IsGenericTypeDefinition ? new Func<Type, bool>(type => this.CheckIfTypeMatchGeneric(baseType, type)) : new Func<Type, bool>(type => this.CheckIfTypeMatch(baseType, type));
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
            });
        }

        private bool CheckIfTypeMatch(Type baseType, Type testingType) {
            return baseType != testingType && !testingType.IsAbstract && baseType.IsAssignableFrom(testingType);
        }

        private bool CheckIfTypeMatchGeneric(Type baseType, Type testingType) {
            return baseType != testingType && !testingType.IsAbstract && testingType.BaseType?.IsGenericType == true && testingType.BaseType.GetGenericTypeDefinition() == baseType;
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