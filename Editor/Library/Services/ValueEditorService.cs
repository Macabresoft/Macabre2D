namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Input;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// An interface for a service which produces value editor controls given an object that contains a
    /// <see cref="DataContractAttribute" />.
    /// </summary>
    public interface IValueEditorService {
        /// <summary>
        /// Creates value editors for an object based on its properties and fields that contain a
        /// <see cref="DataMemberAttribute" />.
        /// </summary>
        /// <param name="editableObject">The editable object for which to make editors..</param>
        /// <param name="typesToIgnore"></param>
        /// <returns>The value editors.</returns>
        IReadOnlyCollection<IValueEditor> CreateEditors(object editableObject, params Type[] typesToIgnore);

        /// <summary>
        /// Gets component editors for the provided <see cref="IGameEntity" />.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="deleteComponentCommand">A command to delete a component from the entity.</param>
        /// <returns>Value editor collections for each component on the entity.</returns>
        IEnumerable<ValueEditorCollection> GetComponentEditors(IGameEntity entity, ICommand deleteComponentCommand);

        /// <summary>
        /// Initializes the value editor service.
        /// </summary>
        /// <param name="enumEditorType">The type of the enum editor.</param>
        void Initialize(Type enumEditorType);

        /// <summary>
        /// Returns the editors to a cache to be reused. Waste not, want not!
        /// </summary>
        /// <param name="editorCollections">The editor collections to return.</param>
        void ReturnEditors(IEnumerable<ValueEditorCollection> editorCollections);
    }

    /// <summary>
    /// A service which produces value editor controls given an object that contains a <see cref="DataContractAttribute" />.
    /// </summary>
    public class ValueEditorService : ReactiveObject, IValueEditorService {
        private readonly IAssemblyService _assemblyService;
        private readonly Dictionary<Type, IList<IValueEditor>> _editorCache = new();
        private Type _enumEditorType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditorService" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        public ValueEditorService(IAssemblyService assemblyService) {
            this._assemblyService = assemblyService;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IValueEditor> CreateEditors(object editableObject, params Type[] typesToIgnore) {
            return this.CreateEditors(string.Empty, editableObject, editableObject, typesToIgnore);
        }

        /// <inheritdoc />
        public IEnumerable<ValueEditorCollection> GetComponentEditors(IGameEntity entity, ICommand deleteComponentCommand) {
            var editors = new List<ValueEditorCollection>();
            foreach (var component in entity.Components) {
                var componentEditors = this.CreateEditors(component);
                if (componentEditors.Any()) {
                    var editorCollection = new ValueEditorCollection(componentEditors, component, component.GetType().Name, deleteComponentCommand);
                    editors.Add(editorCollection);
                }
            }

            return editors;
        }

        public void Initialize(Type enumEditorType) {
            this._enumEditorType = enumEditorType;
        }

        /// <inheritdoc />
        public void ReturnEditors(IEnumerable<ValueEditorCollection> editorCollections) {
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

        private IReadOnlyCollection<IValueEditor> CreateEditors(string currentPath, object editableObject, object originalObject, params Type[] typesToIgnore) {
            var editors = new List<IValueEditor>();
            var members = typesToIgnore != null ? editableObject.GetType().GetAllFieldsAndProperties(typeof(DataMemberAttribute)).Where(x => !typesToIgnore.Contains(x.DeclaringType)) : editableObject.GetType().GetAllFieldsAndProperties(typeof(DataMemberAttribute));

            var membersWithAttributes = members
                .Select(x => new AttributeMemberInfo<DataMemberAttribute>(x, x.GetCustomAttributes(typeof(DataMemberAttribute), false).OfType<DataMemberAttribute>().FirstOrDefault()))
                .OrderBy(x => x.Attribute.Order);

            foreach (var member in membersWithAttributes) {
                var propertyPath = currentPath == string.Empty ? member.MemberInfo.Name : $"{currentPath}.{member.MemberInfo.Name}";
                var memberType = member.MemberInfo.GetMemberReturnType();
                var value = member.MemberInfo.GetValue(editableObject);
                var name = string.IsNullOrEmpty(member.Attribute.Name) ? member.MemberInfo.Name : member.Attribute.Name;
                var editor = this.GetEditorForType(originalObject, value, memberType, propertyPath, name);

                if (editor != null) {
                    editors.Add(editor);
                }
            }

            return editors;
        }

        private IValueEditor GetEditorForType(object originalObject, object value, Type memberType, string propertyPath, string memberName) {
            IValueEditor result = null;

            if (this._editorCache.TryGetValue(memberType, out var editorList)) {
                result = editorList.FirstOrDefault();
            }

            if (result == null) {
                if (memberType.IsEnum) {
                    if (this._enumEditorType != null && Activator.CreateInstance(this._enumEditorType) is IValueEditor editor) {
                        result = editor;
                    }
                }
                else {
                    var editorType = this._assemblyService.LoadFirstType(typeof(IValueEditor<>).MakeGenericType(memberType));
                    if (editorType != null && Activator.CreateInstance(editorType) is IValueEditor editor) {
                        result = editor;
                    }
                }
            }

            if (result != null) {
                result.Initialize(value, memberType, propertyPath, memberName, originalObject);

                if (result is IParentValueEditor parentValueEditor) {
                    parentValueEditor.Initialize(this, this._assemblyService);
                }
            }

            return result;
        }
    }
}