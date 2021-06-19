namespace Macabresoft.Macabre2D.UI.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.UI.Library.Mappers;
    using Macabresoft.Macabre2D.UI.Library.Models;
    using ReactiveUI;

    /// <summary>
    /// An interface for a service which produces value editor controls given an object that contains a
    /// <see cref="DataContractAttribute" />.
    /// </summary>
    public interface IValueEditorService {
        /// <summary>
        /// Creates a single editor collection for an editable object.
        /// </summary>
        /// <param name="editableObject">The editable object.</param>
        /// <param name="name">The name.</param>
        /// <returns>The editor collection.</returns>
        ValueEditorCollection CreateEditor(object editableObject, string name);

        /// <summary>
        /// Creates value editors for an object based on its properties and fields that contain a
        /// <see cref="DataMemberAttribute" />.
        /// </summary>
        /// <param name="editableObject">The editable object for which to make editors..</param>
        /// <param name="typesToIgnore"></param>
        /// <returns>The value editors in collections split by their category.</returns>
        IReadOnlyCollection<ValueEditorCollection> CreateEditors(object editableObject, params Type[] typesToIgnore);

        /// <summary>
        /// Returns the editors to a cache to be reused. Waste not, want not!
        /// </summary>
        /// <param name="editorCollections">The editor collections to return.</param>
        void ReturnEditors(params ValueEditorCollection[] editorCollections);
    }

    /// <summary>
    /// A service which produces value editor controls given an object that contains a <see cref="DataContractAttribute" />.
    /// </summary>
    public class ValueEditorService : ReactiveObject, IValueEditorService {
        private const string DefaultCategoryName = "Uncategorized";
        private readonly IAssemblyService _assemblyService;
        private readonly IValueEditorTypeMapper _typeMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditorService" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <param name="typeMapper">The value editor type mapper.</param>
        public ValueEditorService(IAssemblyService assemblyService, IValueEditorTypeMapper typeMapper) {
            this._assemblyService = assemblyService;
            this._typeMapper = typeMapper;
        }

        /// <inheritdoc />
        public ValueEditorCollection CreateEditor(object editableObject, string name) {
            var editors = this.CreateEditors(string.Empty, editableObject, editableObject);
            return new ValueEditorCollection(editors, editableObject, name);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ValueEditorCollection> CreateEditors(object editableObject, params Type[] typesToIgnore) {
            var valueEditors = this.CreateEditors(string.Empty, editableObject, editableObject, typesToIgnore);
            return valueEditors.GroupBy(x => x.Category).Select(x => new ValueEditorCollection(x, editableObject, x.Key)).ToList();
        }

        /// <inheritdoc />
        public void ReturnEditors(params ValueEditorCollection[] editorCollections) {
            foreach (var editorCollection in editorCollections) {
                editorCollection.Dispose();
            }
        }

        private IReadOnlyCollection<IValueEditor> CreateEditors(string currentPath, object editableObject, object originalObject, params Type[] typesToIgnore) {
            var result = new List<IValueEditor>();
            var editableObjectType = editableObject.GetType();
            var members = typesToIgnore != null ? editableObjectType.GetAllFieldsAndProperties(typeof(DataMemberAttribute)).Where(x => !typesToIgnore.Contains(x.DeclaringType)) : editableObject.GetType().GetAllFieldsAndProperties(typeof(DataMemberAttribute));

            var membersWithAttributes = members
                .Select(x => new AttributeMemberInfo<DataMemberAttribute>(x, x.GetCustomAttribute(typeof(DataMemberAttribute), false) as DataMemberAttribute))
                .OrderBy(x => x.Attribute.Order);

            foreach (var member in membersWithAttributes) {
                var propertyPath = currentPath == string.Empty ? member.MemberInfo.Name : $"{currentPath}.{member.MemberInfo.Name}";
                var memberType = member.MemberInfo.GetMemberReturnType();
                var value = member.MemberInfo.GetValue(editableObject);
                var editors = this.CreateEditorsForMember(originalObject, value, memberType, member, propertyPath);
                result.AddRange(editors);
            }

            return result;
        }

        private ICollection<IValueEditor> CreateEditorsForMember(
            object originalObject,
            object value,
            Type memberType,
            AttributeMemberInfo<DataMemberAttribute> member,
            string propertyPath) {
            var result = new List<IValueEditor>();
            var editorType = this._assemblyService.LoadFirstType(typeof(IValueEditor<>).MakeGenericType(memberType));
            if (editorType != null) {
                var editor = this.CreateValueEditorFromType(editorType, originalObject, value, memberType, member, propertyPath);
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
            else if (!memberType.IsValueType && memberType.GetCustomAttribute(typeof(DataContractAttribute), true) != null) {
                var editors = this.CreateEditors(propertyPath, value, originalObject);
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
            if (Activator.CreateInstance(editorType) is IValueEditor editor) {
                var title = !string.IsNullOrEmpty(member.Attribute.Name) ? member.Attribute.Name : member.MemberInfo.Name;
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
    }
}