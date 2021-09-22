namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Mappers;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// An interface for a service which produces value editor controls given an object that contains a
    /// <see cref="DataContractAttribute" />.
    /// </summary>
    public interface IValueControlService {
        /// <summary>
        /// Creates a single control collection for an object.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="name">The name.</param>
        /// <returns>The editor collection.</returns>
        ValueControlCollection CreateControl(object owner, string name);

        /// <summary>
        /// Creates value controls for an object based on its properties and fields that contain a
        /// <see cref="DataMemberAttribute" />.
        /// </summary>
        /// <param name="owner">The owner for which to make controls.</param>
        /// <param name="typesToIgnore">The types to ignore.</param>
        /// <returns>The value controls in collections split by their category.</returns>
        IReadOnlyCollection<ValueControlCollection> CreateControls(object owner, params Type[] typesToIgnore);

        /// <summary>
        /// Returns the controls and disposes them.
        /// </summary>
        /// <param name="controlCollections">The control collections to return.</param>
        void ReturnControls(params ValueControlCollection[] controlCollections);
    }

    /// <summary>
    /// A service which produces value editor controls given an object that contains a <see cref="DataContractAttribute" />.
    /// </summary>
    public class ValueControlService : ReactiveObject, IValueControlService {
        private const string DefaultCategoryName = "Uncategorized";
        private const string DefaultCategoryNameForInfo = "Info";
        private readonly IAssemblyService _assemblyService;
        private readonly IValueEditorTypeMapper _typeMapper;
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueControlService" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <param name="container">The unity container.</param>
        /// <param name="typeMapper">The value editor type mapper.</param>
        public ValueControlService(IAssemblyService assemblyService, IUnityContainer container, IValueEditorTypeMapper typeMapper) {
            this._assemblyService = assemblyService;
            this._container = container;
            this._typeMapper = typeMapper;
        }

        /// <inheritdoc />
        public ValueControlCollection CreateControl(object owner, string name) {
            var editors = this.CreateControls(string.Empty, owner, owner);
            return new ValueControlCollection(editors, name);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ValueControlCollection> CreateControls(object owner, params Type[] typesToIgnore) {
            var valueEditors = this.CreateControls(string.Empty, owner, owner, typesToIgnore);
            return valueEditors.GroupBy(x => x.Category).Select(x => new ValueControlCollection(x, x.Key)).ToList();
        }

        /// <inheritdoc />
        public void ReturnControls(params ValueControlCollection[] controlCollections) {
            foreach (var editorCollection in controlCollections) {
                editorCollection.Dispose();
            }
        }

        private IReadOnlyCollection<IValueControl> CreateControls(string currentPath, object owner, object originalObject, params Type[] typesToIgnore) {
            var result = new List<IValueControl>();
            var editableObjectType = owner.GetType();
            var members = typesToIgnore != null ? editableObjectType.GetAllFieldsAndProperties(typeof(DataMemberAttribute)).Where(x => !typesToIgnore.Contains(x.DeclaringType)) : owner.GetType().GetAllFieldsAndProperties(typeof(DataMemberAttribute));

            var membersWithAttributes = members
                .Select(x => new AttributeMemberInfo<DataMemberAttribute>(x, x.GetCustomAttribute(typeof(DataMemberAttribute), false) as DataMemberAttribute))
                .OrderBy(x => x.Attribute.Order);

            if (this.TryCreateValueInfo(originalObject, owner, out var info)) {
                result.Add(info);
            }

            foreach (var member in membersWithAttributes) {
                var propertyPath = currentPath == string.Empty ? member.MemberInfo.Name : $"{currentPath}.{member.MemberInfo.Name}";
                var value = member.MemberInfo.GetValue(owner);
                var memberType = member.MemberInfo.GetMemberReturnType();

                if (memberType == typeof(RenderSettings)) {
                    Debugger.Break();
                }

                var editors = this.CreateControlsForMember(originalObject, value, memberType, member, propertyPath);
                result.AddRange(editors);
            }

            return result;
        }

        private ICollection<IValueControl> CreateControlsForMember(
            object originalObject,
            object value,
            Type memberType,
            AttributeMemberInfo<DataMemberAttribute> member,
            string propertyPath) {
            var result = new List<IValueControl>();

            if (this._assemblyService.LoadFirstType(typeof(IValueEditor<>).MakeGenericType(memberType)) is Type memberEditorType) {
                var editor = this.CreateValueEditorFromType(memberEditorType, originalObject, value, memberType, member, propertyPath);
                if (editor != null) {
                    result.Add(editor);
                }
            }
            else if (memberType.IsEnum) {
                if (memberType.GetCustomAttributes<FlagsAttribute>().Any()) {
                    if (this._typeMapper.FlagsEnumEditorType != null) {
                        var editor = this.CreateValueEditorFromType(this._typeMapper.FlagsEnumEditorType, originalObject, value, memberType, member, propertyPath);
                        if (editor != null) {
                            result.Add(editor);
                        }
                    }
                }
                else if (this._typeMapper.EnumEditorType != null) {
                    var editor = this.CreateValueEditorFromType(this._typeMapper.EnumEditorType, originalObject, value, memberType, member, propertyPath);
                    if (editor != null) {
                        result.Add(editor);
                    }
                }
            }
            else if (!memberType.IsValueType) {
                var editors = this.CreateControls(propertyPath, value, originalObject);
                result.AddRange(editors);
            }

            return result;
        }

        private IValueEditor CreateValueEditorFromType(
            Type editorType,
            object originalObject,
            object value,
            Type memberType,
            AttributeMemberInfo<DataMemberAttribute> member,
            string propertyPath) {
            if (this._container.Resolve(editorType) is IValueEditor editor) {
                var title = GetTitle(member);
                editor.Initialize(value, memberType, propertyPath, title, originalObject);
                editor.Category = DefaultCategoryName;

                if (member.MemberInfo.GetCustomAttribute(typeof(CategoryAttribute), false) is CategoryAttribute memberCategory) {
                    editor.Category = memberCategory.Category;
                }
                else if (member.MemberInfo.DeclaringType != null) {
                    if (Attribute.GetCustomAttribute(member.MemberInfo.DeclaringType, typeof(CategoryAttribute), false) is CategoryAttribute classCategory) {
                        editor.Category = classCategory.Category;
                    }
                    else {
                        editor.Category = member.MemberInfo.DeclaringType.Name;
                    }
                }

                return editor;
            }

            return null;
        }

        private static string GetTitle(AttributeMemberInfo<DataMemberAttribute> member) {
            return !string.IsNullOrEmpty(member.Attribute.Name) ? member.Attribute.Name : Regex.Replace(member.MemberInfo.Name, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }

        private bool TryCreateValueInfo(object originalOwner, object value, out IValueControl info) {
            info = null;

            if (value?.GetType() is Type ownerType) {
                var controlType = this._assemblyService.LoadFirstType(typeof(IValueInfo<>).MakeGenericType(ownerType));

                if (controlType != null && !controlType.IsAssignableTo(typeof(IValueEditor)) && this._container.Resolve(controlType) is IValueControl control) {
                    info = control;
                    control.Initialize(value, ownerType, null, DefaultCategoryNameForInfo, originalOwner);

                    if (Attribute.GetCustomAttribute(ownerType, typeof(DataContractAttribute), false) is DataContractAttribute dataContractAttribute && !string.IsNullOrEmpty(dataContractAttribute.Name)) {
                        control.Category = $"{dataContractAttribute.Name} {DefaultCategoryNameForInfo}";
                    }
                    else {
                        control.Category = $"{DefaultCategoryNameForInfo} ({ownerType.Name})";
                    }
                }
            }

            return info != null;
        }
    }
}