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
                    Value = value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(string)) {
                var editor = new StringEditor {
                    Value = (string)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(int)) {
                var editor = new IntEditor {
                    Value = (int)value
                };

                result = editor;
            }
            else if (memberType == typeof(float)) {
                var editor = new FloatEditor {
                    Value = (float)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(bool)) {
                var editor = new BoolEditor {
                    Value = (bool)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Vector2)) {
                var editor = new VectorEditor {
                    Value = (Vector2)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Microsoft.Xna.Framework.Point)) {
                var editor = new PointEditor {
                    Value = (Microsoft.Xna.Framework.Point)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Microsoft.Xna.Framework.Color)) {
                var editor = new ColorEditor {
                    Value = (Microsoft.Xna.Framework.Color)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Sprite)) {
                var editor = new SpriteEditor {
                    Value = value as Sprite
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(AudioClip)) {
                var editor = new AudioClipEditor {
                    Value = value as AudioClip
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(SpriteAnimation)) {
                var editor = new SpriteAnimationEditor {
                    Value = value as SpriteAnimation
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Font)) {
                var editor = new FontEditor {
                    Value = value as Font
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(Collider)) {
                var editor = new ColliderEditor {
                    Value = (Collider)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(LineCollider)) {
                var editor = new LineColliderEditor {
                    Value = (LineCollider)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType == typeof(RectangleCollider)) {
                var editor = new RectangleColliderEditor {
                    Value = (RectangleCollider)value
                };

                await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                result = editor;
            }
            else if (memberType.IsSubclassOf(typeof(BaseComponent))) {
                // TODO: Allow user to select a component.
            }
            else {
                // TODO: I don't know, this should probably work when value is null
                if (value != null) {
                    var editor = new GenericValueEditor {
                        DeclaringType = declaringTypeToIgnore,
                        Value = value
                    };

                    await editor.Initialize(memberType, originalObject, propertyPath, memberName);
                    result = editor;
                }
            }

            return result;
        }
    }
}