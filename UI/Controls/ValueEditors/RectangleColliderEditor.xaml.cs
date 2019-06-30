namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework.Physics;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Microsoft.Xna.Framework;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public partial class RectangleColliderEditor : NamedValueEditor<RectangleCollider> {
        private const string OffsetFieldName = "_offset";

        public RectangleColliderEditor() {
            this.InitializeComponent();
        }

        public Vector2 Offset {
            get {
                if (this.Value != null) {
                    return (Vector2)this.Value.GetProperty(RectangleColliderEditor.OffsetFieldName);
                }

                return Vector2.Zero;
            }

            set {
                this.UpdateProperty(RectangleColliderEditor.OffsetFieldName, this.Offset, value);
            }
        }

        public float RectangleHeight {
            get {
                if (this.Value != null) {
                    return this.Value.Height;
                }

                return 0f;
            }

            set {
                this.UpdateProperty(nameof(this.Value.Height), this.RectangleHeight, value);
            }
        }

        public float RectangleWidth {
            get {
                if (this.Value != null) {
                    return this.Value.Width;
                }

                return 0f;
            }

            set {
                this.UpdateProperty(nameof(this.Value.Width), this.RectangleWidth, value);
            }
        }

        protected override void OnValueChanged(RectangleCollider newValue, RectangleCollider oldValue, DependencyObject d) {
            base.OnValueChanged(newValue, oldValue, d);
            this.RaisePropertyChanged(nameof(this.Offset));
            this.RaisePropertyChanged(nameof(this.RectangleWidth));
            this.RaisePropertyChanged(nameof(this.RectangleHeight));
        }

        private void UpdateProperty(string propertyPath, object originalValue, object newValue, [CallerMemberName] string localPropertyName = "") {
            var hasChanges = this._sceneService.HasChanges;
            var undoCommand = new UndoCommand(
                () => {
                    this.Value.SetProperty(propertyPath, newValue);
                    this.RaisePropertyChanged(localPropertyName);
                    this._sceneService.HasChanges = true;
                },
                () => {
                    this.Value.SetProperty(propertyPath, originalValue);
                    this.RaisePropertyChanged(localPropertyName);
                    this._sceneService.HasChanges = hasChanges;
                });

            this._undoService.Do(undoCommand);
        }
    }
}