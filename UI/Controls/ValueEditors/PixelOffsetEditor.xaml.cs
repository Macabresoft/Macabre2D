namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public partial class PixelOffsetEditor : NamedValueEditor<PixelOffset> {

        public PixelOffsetEditor() {
            this.InitializeComponent();
        }

        public Vector2 Amount {
            get {
                if (this.Value != null) {
                    return this.Value.Amount;
                }

                return Vector2.Zero;
            }

            set {
                if (this.Value != null) {
                    this.UpdateProperty(nameof(PixelOffset.Amount), this.Amount, value, nameof(this.Amount), nameof(this.OffsetType));
                }
            }
        }

        public PixelOffsetType OffsetType {
            get {
                if (this.Value != null) {
                    return this.Value.Type;
                }

                return PixelOffsetType.Custom;
            }

            set {
                if (this.Value != null) {
                    this.UpdateProperty(nameof(PixelOffset.Type), this.OffsetType, value, nameof(this.Amount), nameof(this.OffsetType));
                }
            }
        }

        public IReadOnlyCollection<PixelOffsetType> OffsetTypes {
            get {
                return Enum.GetValues(typeof(PixelOffsetType)).Cast<PixelOffsetType>().ToList();
            }
        }

        protected override void OnValueChanged(PixelOffset newValue, PixelOffset oldValue, DependencyObject d) {
            base.OnValueChanged(newValue, oldValue, d);
            this.RaisePropertyChanged(nameof(this.Amount));
            this.RaisePropertyChanged(nameof(this.OffsetType));
        }
    }
}