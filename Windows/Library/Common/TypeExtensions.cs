namespace Macabre2D.Engine.Windows.Common {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions {

        public static IEnumerable<MethodInfo> GetAllMethods(this Type owner) {
            var result = new List<MethodInfo>();
            var currentType = owner;

            while (currentType != null) {
                var methods = currentType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                result.AddRange(methods.Where(x => !result.Any(y => y.MetadataToken == x.MetadataToken)));
                currentType = currentType.BaseType;
            }

            return result;
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this Type owner, Type methodAttribute) {
            return owner.GetAllMethods().Where(method => Attribute.IsDefined(method, methodAttribute));
        }

        public static IEnumerable<MemberInfo> GetFieldsAndProperties(this Type owner, Type memberAttribute) {
            return owner.GetFieldsAndProperties().Where(prop => Attribute.IsDefined(prop, memberAttribute));
        }

        public static IEnumerable<MemberInfo> GetFieldsAndProperties(this Type owner) {
            var result = new List<MemberInfo>();
            var currentType = owner;

            while (currentType != null) {
                var fields = currentType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                var properties = currentType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                result.AddRange(fields.Where(x => !result.Any(y => y.MetadataToken == x.MetadataToken)));
                result.AddRange(properties.Where(x => !result.Any(y => y.MetadataToken == x.MetadataToken)));
                currentType = currentType.BaseType;
            }

            return result;
        }

        public static MemberInfo GetMemberInfoForFieldOrProperty(this Type ownerType, string name) {
            var currentType = ownerType;
            MemberInfo result = null;
            while (currentType != null && result == null) {
                result = (MemberInfo)currentType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) ??
                currentType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                currentType = currentType.BaseType;
            }

            return result;
        }

        public static Type GetMemberReturnType(this MemberInfo memberInfo) {
            Type type = null;

            switch (memberInfo.MemberType) {
                case MemberTypes.Constructor:
                    type = memberInfo.DeclaringType;
                    break;

                case MemberTypes.Event:
                    if (memberInfo is EventInfo eventInfo) {
                        type = eventInfo.EventHandlerType;
                    }
                    break;

                case MemberTypes.Field:
                    if (memberInfo is FieldInfo fieldInfo) {
                        type = fieldInfo.FieldType;
                    }
                    break;

                case MemberTypes.Method:
                    if (memberInfo is MethodInfo methodInfo) {
                        type = methodInfo.ReturnType;
                    }
                    break;

                case MemberTypes.Property:
                    if (memberInfo is PropertyInfo propertyInfo) {
                        type = propertyInfo.PropertyType;
                    }
                    break;

                case MemberTypes.NestedType:
                case MemberTypes.TypeInfo:
                    if (memberInfo is TypeInfo typeInfo) {
                        type = typeInfo.UnderlyingSystemType;
                    }
                    break;
            }

            if (type == null) {
                throw new NotSupportedException("You were doing something totally crazy and unexpected.");
            }

            return type;
        }

        public static object GetValue(this MemberInfo memberInfo, object owner) {
            if (memberInfo is PropertyInfo propertyInfo) {
                return propertyInfo.GetValue(owner);
            }
            else if (memberInfo is FieldInfo fieldInfo) {
                return fieldInfo.GetValue(owner);
            }

            throw new NotSupportedException("Can only get value for member types that are properties or fields.");
        }

        public static void SetValue(this MemberInfo memberInfo, object owner, object value) {
            if (memberInfo is PropertyInfo propertyInfo) {
                propertyInfo.SetValue(owner, value, null);
            }
            else if (memberInfo is FieldInfo fieldInfo) {
                fieldInfo.SetValue(owner, value);
            }
            else {
                throw new NotSupportedException("Can only set value for member types that are properties or fields.");
            }
        }
    }
}