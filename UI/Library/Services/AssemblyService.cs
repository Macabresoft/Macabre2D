namespace Macabre2D.UI.Library.Services {

    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public interface IAssemblyService {

        Task<Type> LoadFirstType(Type baseType);

        Task<IList<Type>> LoadTypes(Type baseType);
    }

    public sealed class AssemblyService : IAssemblyService {

        public async Task<Type> LoadFirstType(Type baseType) {
            return await Task.Run(() => {
                Type resultType = null;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var filter = baseType.IsGenericTypeDefinition ? new Func<Type, bool>(type => this.CheckIfTypeMatchGeneric(baseType, type)) : new Func<Type, bool>(type => this.CheckIfTypeMatch(baseType, type));
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
            });
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