namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework.Physics;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Microsoft.Xna.Framework;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public partial class LineColliderEditor : NamedValueEditor<LineCollider> {
        private const string OffsetFieldName = "_offset";

        public LineColliderEditor() : base() {
            this.InitializeComponent();
        }

        public Vector2 End {
            get {
                if (this.Value != null) {
                    return this.Value.End;
                }

                return Vector2.Zero;
            }

            set {
                this.UpdateProperty(nameof(this.Value.End), this.End, value);
            }
        }

        public Vector2 Offset {
            get {
                if (this.Value != null) {
                    return (Vector2)this.Value.GetProperty(LineColliderEditor.OffsetFieldName);
                }

                return Vector2.Zero;
            }

            set {
                this.UpdateProperty(LineColliderEditor.OffsetFieldName, this.Offset, value);
            }
        }

        public Vector2 Start {
            get {
                if (this.Value != null) {
                    return this.Value.Start;
                }

                return Vector2.Zero;
            }

            set {
                this.UpdateProperty(nameof(this.Value.Start), this.Start, value);
            }
        }

        protected override void OnValueChanged(LineCollider newValue, LineCollider oldValue, DependencyObject d) {
            base.OnValueChanged(newValue, oldValue, d);
            this.RaisePropertyChanged(nameof(this.Offset));
            this.RaisePropertyChanged(nameof(this.Start));
            this.RaisePropertyChanged(nameof(this.End));
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