namespace Macabre2D.UI.Library.Services {

    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.Controls.ValueEditors;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.ServiceInterfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Windows;

    public sealed class ValueEditorService : IValueEditorService {
        private readonly IAssemblyService _assemblyService;

        public ValueEditorService(IAssemblyService assemblyService) {
            this._assemblyService = assemblyService;
        }

        public async Task<DependencyObject> CreateEditor(object editableObject, string name, Type declaringType) {
            return editableObject == null ? null : await this.GetEditorForType(editableObject, editableObject, editableObject.GetType(), string.Empty, name, declaringType);
        }

        public async Task<IList<DependencyObject>> CreateEditors(object editableObject, Type declaringType, params Type[] typesToIgnore) {
            return await this.CreateEditors(string.Empty, editableObject, editableObject, declaringType, typesToIgnore);
        }

        private async Task<IList<DependencyObject>> CreateEditors(string currentPath, object editableObject, object originalObject, Type declaringType, params Type[] typesToIgnore) {
            var editors = new List<DependencyObject>();
            var members = typesToIgnore != null ?
                editableObject.GetType().GetFieldsAndProperties(typeof(DataMemberAttribute)).Where(x => !typesToIgnore.Contains(x.DeclaringType)) :
                editableObject.GetType().GetFieldsAndProperties(typeof(DataMemberAttribute));

            var membersWithAttributes = members
                .Select(x => new MemberInfoAttribute<DataMemberAttribute>(x, x.GetCustomAttributes(typeof(DataMemberAttribute), false).OfType<DataMemberAttribute>().FirstOrDefault()))
                .OrderBy(x => x.Attribute.Order);

            foreach (var member in membersWithAttributes) {
                var propertyPath = currentPath == string.Empty ? member.MemberInfo.Name : $"{currentPath}.{member.MemberInfo.Name}";
                var memberType = member.MemberInfo.GetMemberReturnType();
                var value = member.MemberInfo.GetValue(editableObject);
                var name = string.IsNullOrEmpty(member.Attribute.Name) ? member.MemberInfo.Name : member.Attribute.Name;
                var editor = await this.GetEditorForType(originalObject, value, memberType, propertyPath, name, declaringType);
                editors.Add(editor);
            }

            return editors;
        }

        private async Task<DependencyObject> GetEditorForType(object originalObject, object value, Type memberType, string propertyPath, string memberName, Type declaringType) {
            DependencyObject result = null;

            var editorType = await this._assemblyService.LoadFirstType(typeof(INamedValueEditor<>).MakeGenericType(memberType));
            if (editorType != null && Activator.CreateInstance(editorType) is INamedValueEditor editor && editor is DependencyObject dependencyObject) {
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = dependencyObject;
            }
            else if (memberType.IsEnum) {
                var enumEditor = new EnumEditor();
                await enumEditor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = enumEditor;
            }
            else if (value != null) { // TODO: I don't know, this should probably work when value is null. Maybe it already does?
                var genericEditor = new GenericValueEditor {
                    DeclaringType = declaringType
                };

                await genericEditor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = genericEditor;
            }

            return result;
        }
    }
}