namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Mappers;
    using Macabresoft.Macabre2D.Editor.Library.Models;
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
        private readonly Dictionary<Type, IList<IValueEditor>> _editorCache = new();
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
            var collections = this.CreateEditors(string.Empty, editableObject, editableObject);
            return new ValueEditorCollection(collections.SelectMany(x => x.ValueEditors), editableObject, name);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ValueEditorCollection> CreateEditors(object editableObject, params Type[] typesToIgnore) {
            return this.CreateEditors(string.Empty, editableObject, editableObject, typesToIgnore);
        }

        /// <inheritdoc />
        public void ReturnEditors(params ValueEditorCollection[] editorCollections) {
            foreach (var editorCollection in editorCollections) {
                foreach (var valueEditor in editorCollection.ValueEditors) {
                    if (this._editorCache.TryGetValue(valueEditor.ValueType, out var valueEditorCache)) {
                        valueEditorCache.Add(valueEditor);
                    }
                    else {
                        var newEditorCache = new List<IValueEditor> {
                            valueEditor
                        };

                        this._editorCache[valueEditor.ValueType] = newEditorCache;
                    }
                }

                editorCollection.Dispose();
            }
        }

        private IReadOnlyCollection<ValueEditorCollection> CreateEditors(string currentPath, object editableObject, object originalObject, params Type[] typesToIgnore) {
            var categoryToEditors = new Dictionary<string, IList<IValueEditor>>();
            var editableObjectType = editableObject.GetType();
            var members = typesToIgnore != null ? editableObjectType.GetAllFieldsAndProperties(typeof(DataMemberAttribute)).Where(x => !typesToIgnore.Contains(x.DeclaringType)) : editableObject.GetType().GetAllFieldsAndProperties(typeof(DataMemberAttribute));

            var membersWithAttributes = members
                .Select(x => new AttributeMemberInfo<DataMemberAttribute>(x, x.GetCustomAttributes(typeof(DataMemberAttribute), false).OfType<DataMemberAttribute>().FirstOrDefault()))
                .OrderBy(x => x.Attribute.Order);

            foreach (var member in membersWithAttributes) {
                var propertyPath = currentPath == string.Empty ? member.MemberInfo.Name : $"{currentPath}.{member.MemberInfo.Name}";
                var memberType = member.MemberInfo.GetMemberReturnType();
                var value = member.MemberInfo.GetValue(editableObject);
                var editor = this.GetEditorForType(originalObject, value, memberType, member.MemberInfo, propertyPath);

                if (editor != null) {
                    var category = DefaultCategoryName;

                    if (member.MemberInfo.GetCustomAttribute(typeof(CategoryAttribute), false) is CategoryAttribute memberCategory) {
                        category = memberCategory.Category;
                    }
                    else if (Attribute.GetCustomAttribute(editableObjectType, typeof(CategoryAttribute)) is CategoryAttribute classCategory) {
                        category = classCategory.Category;
                    }

                    if (categoryToEditors.TryGetValue(category, out var editors)) {
                        editors.Add(editor);
                    }
                    else {
                        editors = new List<IValueEditor> {
                            editor
                        };

                        categoryToEditors.Add(category, editors);
                    }
                }
            }

            return categoryToEditors.Select(x => new ValueEditorCollection(x.Value, editableObject, x.Key)).ToList();
        }

        private IValueEditor GetEditorForType(object originalObject, object value, Type memberType, MemberInfo memberInfo, string propertyPath) {
            IValueEditor result = null;

            if (this._editorCache.TryGetValue(memberType, out var editorList)) {
                result = editorList.FirstOrDefault();
            }

            if (result == null) {
                var editorType = this._assemblyService.LoadFirstType(typeof(IValueEditor<>).MakeGenericType(memberType));
                if (editorType != null) {
                    if (Activator.CreateInstance(editorType) is IValueEditor editor) {
                        result = editor;
                    }
                }
                else if (memberType.IsEnum) {
                    if (memberType.GetCustomAttributes<FlagsAttribute>().Any()) {
                        if (this._typeMapper.FlagsEnumEditorType != null) {
                            // TODO: pull from cache
                            result = Activator.CreateInstance(this._typeMapper.FlagsEnumEditorType) as IValueEditor;
                        }
                    }
                    else {
                        if (this._typeMapper.EnumEditorType != null) {
                            // TODO: pull from cache
                            result = Activator.CreateInstance(this._typeMapper.EnumEditorType) as IValueEditor;
                        }
                    }
                }
                else if (!memberType.IsValueType && this._typeMapper.GenericEditorType != null && memberType.GetCustomAttributes(typeof(DataContractAttribute), true).Any()) {
                    // TODO: pull from cache
                    result = Activator.CreateInstance(this._typeMapper.GenericEditorType) as IValueEditor;
                }
            }

            if (result != null) {
                var title = memberType.GetPropertyDisplayName(memberInfo.Name);
                result.Initialize(value, memberType, propertyPath, title, originalObject);

                if (result is IParentValueEditor parentValueEditor) {
                    parentValueEditor.Initialize(this, this._assemblyService);
                }
            }

            return result;
        }
    }
}