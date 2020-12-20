namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;
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
        /// <returns></returns>
        Task<IReadOnlyCollection<IValueEditor>> CreateEditors(object editableObject, params Type[] typesToIgnore);
    }

    /// <summary>
    /// A service which produces value editor controls given an object that contains a <see cref="DataContractAttribute" />.
    /// </summary>
    public class ValueEditorService : ReactiveObject, IValueEditorService {
        private readonly IAssemblyService _assemblyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditorService" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        public ValueEditorService(IAssemblyService assemblyService) {
            this._assemblyService = assemblyService;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IValueEditor>> CreateEditors(object editableObject, params Type[] typesToIgnore) {
            return await this.CreateEditors(string.Empty, editableObject, editableObject, typesToIgnore);
        }

        private async Task<IReadOnlyCollection<IValueEditor>> CreateEditors(string currentPath, object editableObject, object originalObject, params Type[] typesToIgnore) {
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
                var editor = await this.GetEditorForType(originalObject, value, memberType, propertyPath, name);
                editors.Add(editor);
            }

            return editors;
        }

        private async Task<IValueEditor> GetEditorForType(object originalObject, object value, Type memberType, string propertyPath, string memberName) {
            IValueEditor result = null;

            var editorType = await this._assemblyService.LoadFirstType(typeof(IValueEditor<>).MakeGenericType(memberType));
            if (editorType != null && Activator.CreateInstance(editorType) is IValueEditor editor) {
                editor.Initialize(value, originalObject, propertyPath, memberName);
                result = editor;
            }
            // else if (memberType.IsEnum) {
            //     var enumEditor = new EnumEditor();
            //     await enumEditor.Initialize(value, memberType, originalObject, propertyPath, memberName);
            //     result = enumEditor;
            // }
            // else {
            //     result = await this.GetSpecializedEditor(originalObject, value, memberType, propertyPath, memberName, declaringType);
            // }

            return result;
        }
    }
}