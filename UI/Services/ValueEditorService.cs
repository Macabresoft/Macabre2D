namespace Macabre2D.UI.Services {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.Framework.Audio;
    using Macabre2D.Framework.Physics;
    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Controls.ValueEditors;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
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
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<object>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(string)) {
                var editor = new StringEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (string)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<string>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(int)) {
                var editor = new IntEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (int)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<int>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(float)) {
                var editor = new FloatEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (float)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<float>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(bool)) {
                var editor = new BoolEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (bool)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<bool>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(Vector2)) {
                var editor = new VectorEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (Vector2)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<Vector2>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(Microsoft.Xna.Framework.Point)) {
                var editor = new PointEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (Microsoft.Xna.Framework.Point)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<Microsoft.Xna.Framework.Point>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(Microsoft.Xna.Framework.Color)) {
                var editor = new ColorEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (Microsoft.Xna.Framework.Color)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<Microsoft.Xna.Framework.Color>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(Sprite)) {
                var editor = new SpriteEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value as Sprite
                };

                if (value is Sprite sprite) {
                    var spriteWrappers = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<SpriteWrapper>();
                    editor.SpriteWrapper = spriteWrappers.FirstOrDefault(x => x.Sprite.Id == sprite.Id);
                }

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<Sprite>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                editor.SelectSpriteCommand = new RelayCommand(() => {
                    var asset = this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, AssetType.Image | AssetType.Sprite, AssetType.Sprite);
                    if (asset is SpriteWrapper spriteWrapper) {
                        editor.SpriteWrapper = spriteWrapper;
                    }
                }, true);

                result = editor;
            }
            else if (memberType == typeof(AudioClip)) {
                var editor = new AudioClipEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value as AudioClip
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<AudioClip>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                editor.SelectAudioClipCommand = new RelayCommand(() => {
                    var asset = this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, AssetType.Audio, AssetType.Audio);
                    if (asset is AudioAsset audioAsset) {
                        editor.Value = audioAsset.AudioClip;
                    }
                }, true);

                result = editor;
            }
            else if (memberType == typeof(SpriteAnimation)) {
                var editor = new SpriteAnimationEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value as SpriteAnimation
                };

                if (value is SpriteAnimation animation) {
                    var spriteAnimationAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<SpriteAnimationAsset>();
                    editor.Asset = spriteAnimationAssets.FirstOrDefault(x => x.SavableValue.Id == animation.Id);
                }

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<SpriteAnimation>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                editor.SelectSpriteAnimationCommand = new RelayCommand(() => {
                    var asset = this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, AssetType.SpriteAnimation, AssetType.SpriteAnimation);
                    if (asset is SpriteAnimationAsset spriteAnimationAsset) {
                        editor.Asset = spriteAnimationAsset;
                        editor.Value = spriteAnimationAsset.SavableValue;
                    }
                }, true);

                result = editor;
            }
            else if (memberType == typeof(Font)) {
                var editor = new FontEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = value as Font
                };

                if (value is Font font) {
                    var fontAssets = this._projectService.CurrentProject.AssetFolder.GetAssetsOfType<FontAsset>();
                    editor.Asset = fontAssets.FirstOrDefault(x => x.SavableValue.Id == font.Id);
                }

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<Font>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                editor.SelectFontCommand = new RelayCommand(() => {
                    var asset = this._dialogService.ShowSelectAssetDialog(this._projectService.CurrentProject, AssetType.Font, AssetType.Font);
                    if (asset is FontAsset fontAsset) {
                        editor.Asset = fontAsset;
                        editor.Value = fontAsset.SavableValue;
                    }
                }, true);

                result = editor;
            }
            else if (memberType == typeof(Collider)) {
                var colliderTypes = await this._assemblyService.LoadTypes(typeof(Collider));
                colliderTypes.Remove(typeof(PolygonCollider)); // TODO: Eventually allow PolygonCollider.
                var editor = new ColliderEditor {
                    ColliderTypes = colliderTypes,
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (Collider)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<Collider>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(LineCollider)) {
                var colliderTypes = await this._assemblyService.LoadTypes(typeof(Collider));
                var editor = new LineColliderEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (LineCollider)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<LineCollider>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                result = editor;
            }
            else if (memberType == typeof(RectangleCollider)) {
                var colliderTypes = await this._assemblyService.LoadTypes(typeof(Collider));
                var editor = new RectangleColliderEditor {
                    PropertyName = propertyPath,
                    Title = memberName,
                    Value = (RectangleCollider)value
                };

                editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<RectangleCollider>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
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
                        PropertyName = propertyPath,
                        Title = memberName,
                        Value = value
                    };

                    editor.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<object>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, originalObject, editor), true);
                    result = editor;
                }
            }

            return result;
        }

        private void UpdateProperty<T>(string propertyPath, T originalValue, T newValue, object objectToUpdate, INamedValueEditor<T> editor) {
            if ((originalValue == null && newValue != null) || !originalValue.Equals(newValue)) {
                var undoCommand = new UndoCommand(
                    () => {
                        this.UpdatePropertyWithNotification(propertyPath, newValue, objectToUpdate);
                        editor.Value = newValue;
                    },
                    () => {
                        this.UpdatePropertyWithNotification(propertyPath, originalValue, objectToUpdate);
                        editor.Value = originalValue;
                    });

                this._undoService.Do(undoCommand);
            }
        }

        private void UpdatePropertyWithNotification(string propertyPath, object value, object objectToUpdate) {
            objectToUpdate.SetProperty(propertyPath, value);
            this._sceneService.HasChanges = true;
            if (objectToUpdate is NotifyPropertyChanged notifyPropertyChanged) {
                notifyPropertyChanged.RaisePropertyChanged(propertyPath);
            }
        }
    }
}