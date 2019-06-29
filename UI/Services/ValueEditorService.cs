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
        private readonly IDialogService _dialogService;
        private readonly IProjectService _projectService;
        private readonly ISceneService _sceneService;
        private readonly IUndoService _undoService;

        public ValueEditorService(
            IAssemblyService assemblyService,
            IDialogService dialogService,
            IProjectService projectService,
            ISceneService sceneService,
            IUndoService undoService) {
            this._assemblyService = assemblyService;
            this._dialogService = dialogService;
            this._projectService = projectService;
            this._sceneService = sceneService;
            this._undoService = undoService;
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
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value
                };

                result = editor;
            }
            else if (memberType == typeof(string)) {
                var editor = new StringEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (string)value
                };

                result = editor;
            }
            else if (memberType == typeof(int)) {
                var editor = new IntEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (int)value
                };

                result = editor;
            }
            else if (memberType == typeof(float)) {
                var editor = new FloatEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (float)value
                };

                result = editor;
            }
            else if (memberType == typeof(bool)) {
                var editor = new BoolEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (bool)value
                };

                result = editor;
            }
            else if (memberType == typeof(Vector2)) {
                var editor = new VectorEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (Vector2)value
                };

                result = editor;
            }
            else if (memberType == typeof(Microsoft.Xna.Framework.Point)) {
                var editor = new PointEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (Microsoft.Xna.Framework.Point)value
                };

                result = editor;
            }
            else if (memberType == typeof(Microsoft.Xna.Framework.Color)) {
                var editor = new ColorEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (Microsoft.Xna.Framework.Color)value
                };

                result = editor;
            }
            else if (memberType == typeof(Sprite)) {
                var editor = new SpriteEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value as Sprite
                };

                result = editor;
            }
            else if (memberType == typeof(AudioClip)) {
                var editor = new AudioClipEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value as AudioClip
                };

                result = editor;
            }
            else if (memberType == typeof(SpriteAnimation)) {
                var editor = new SpriteAnimationEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value as SpriteAnimation
                };

                result = editor;
            }
            else if (memberType == typeof(Font)) {
                var editor = new FontEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value as Font
                };

                result = editor;
            }
            else if (memberType == typeof(Collider)) {
                var colliderTypes = await this._assemblyService.LoadTypes(typeof(Collider));
                colliderTypes.Remove(typeof(PolygonCollider)); // TODO: Eventually allow PolygonCollider.
                var editor = new ColliderEditor {
                    ColliderTypes = colliderTypes,
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (Collider)value
                };

                result = editor;
            }
            else if (memberType == typeof(LineCollider)) {
                var editor = new LineColliderEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (LineCollider)value
                };

                result = editor;
            }
            else if (memberType == typeof(RectangleCollider)) {
                var editor = new RectangleColliderEditor {
                    Owner = originalObject,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (RectangleCollider)value
                };

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
                        Owner = originalObject,
                        PropertyName = propertyPath,
                        Title = memberName,
                        Value = value
                    };

                    result = editor;
                }
            }

            return result;
        }
    }
}