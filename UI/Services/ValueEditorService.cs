namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Audio;
    using Macabre2D.Framework.Physics;
    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Controls.ValueEditors;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
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

        public async Task<DependencyObject> CreateEditor(object editableObject, string name, Type declaringTypeToIgnore) {
            return editableObject == null ? null : await this.GetEditorForType(editableObject, editableObject, editableObject.GetType(), string.Empty, name, declaringTypeToIgnore);
        }

        public async Task<IEnumerable<DependencyObject>> CreateEditors(object editableObject, Type declaringTypeToIgnore) {
            return await this.CreateEditors(string.Empty, editableObject, editableObject, declaringTypeToIgnore);
        }

        private async Task<IEnumerable<DependencyObject>> CreateEditors(string currentPath, object editableObject, object originalObject, Type declaringTypeToIgnore) {
            var editors = new List<DependencyObject>();
            var members = declaringTypeToIgnore != null ?
                editableObject.GetType().GetFieldsAndProperties(typeof(DataMemberAttribute)).Where(x => x.DeclaringType != declaringTypeToIgnore) :
                editableObject.GetType().GetFieldsAndProperties(typeof(DataMemberAttribute));

            foreach (var member in members) {
                var propertyPath = currentPath == string.Empty ? member.Name : $"{currentPath}.{member.Name}";
                var memberType = member.GetMemberReturnType();
                var value = member.GetValue(editableObject);
                var editor = await this.GetEditorForType(originalObject, value, memberType, propertyPath, member.Name, declaringTypeToIgnore);
                editors.Add(editor);
            }

            return editors;
        }

        private async Task<DependencyObject> GetEditorForType(object originalObject, object value, Type memberType, string propertyPath, string memberName, Type declaringTypeToIgnore) {
            DependencyObject result = null;

            if (memberType.IsEnum) {
                var editor = new EnumEditor {
                    EnumType = memberType,
                };

                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(string)) {
                var editor = new StringEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(int)) {
                var editor = new IntEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(float)) {
                var editor = new FloatEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(bool)) {
                var editor = new BoolEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Vector2)) {
                var editor = new VectorEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Microsoft.Xna.Framework.Point)) {
                var editor = new PointEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Microsoft.Xna.Framework.Color)) {
                var editor = new ColorEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Sprite)) {
                var editor = new SpriteEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(AudioClip)) {
                var editor = new AudioClipEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(SpriteAnimation)) {
                var editor = new SpriteAnimationEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Font)) {
                var editor = new FontEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Collider)) {
                var editor = new ColliderEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(LineCollider)) {
                var editor = new LineColliderEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(RectangleCollider)) {
                var editor = new RectangleColliderEditor();
                await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType.IsSubclassOf(typeof(BaseComponent))) {
                // TODO: Allow user to select a component.
            }
            else {
                // TODO: I don't know, this should probably work when value is null
                if (value != null) {
                    var editor = new GenericValueEditor {
                        DeclaringType = declaringTypeToIgnore
                    };

                    await editor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                    result = editor;
                }
            }

            return result;
        }
    }
}