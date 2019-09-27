namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public partial class RenderSettingsEditor : NamedValueEditor<RenderSettings>, ISeparatedValueEditor {

        public static readonly DependencyProperty ShowBottomSeparatorProperty = DependencyProperty.Register(
            nameof(ShowBottomSeparator),
            typeof(bool),
            typeof(RenderSettingsEditor),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTopSeparatorProperty = DependencyProperty.Register(
            nameof(ShowTopSeparator),
            typeof(bool),
            typeof(RenderSettingsEditor),
            new PropertyMetadata(true));

        public RenderSettingsEditor() {
            this.InitializeComponent();
        }

        public bool FlipHorizontal {
            get {
                return this.Value != null ? this.Value.FlipHorizontal : false;
            }

            set {
                if (this.Value != null) {
                    this.UpdateProperty(nameof(RenderSettings.FlipHorizontal), this.FlipHorizontal, value, nameof(this.FlipHorizontal));
                }
            }
        }

        public bool FlipVertical {
            get {
                return this.Value != null ? this.Value.FlipVertical : false;
            }

            set {
                if (this.Value != null) {
                    this.UpdateProperty(nameof(RenderSettings.FlipVertical), this.FlipVertical, value, nameof(this.FlipVertical));
                }
            }
        }

        public Vector2 Offset {
            get {
                if (this.Value != null) {
                    return this.Value.Offset;
                }

                return Vector2.Zero;
            }

            set {
                if (this.Value != null) {
                    this.UpdateProperty(nameof(RenderSettings.Offset), this.Offset, value, nameof(this.Offset), nameof(this.OffsetType));
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
                    this.UpdateProperty(nameof(RenderSettings.Type), this.OffsetType, value, nameof(this.Offset), nameof(this.OffsetType));
                }
            }
        }

        public IReadOnlyCollection<PixelOffsetType> OffsetTypes {
            get {
                return Enum.GetValues(typeof(PixelOffsetType)).Cast<PixelOffsetType>().ToList();
            }
        }

        public bool ShowBottomSeparator {
            get { return (bool)this.GetValue(ShowBottomSeparatorProperty); }
            set { this.SetValue(ShowBottomSeparatorProperty, value); }
        }

        public bool ShowTopSeparator {
            get { return (bool)this.GetValue(ShowTopSeparatorProperty); }
            set { this.SetValue(ShowTopSeparatorProperty, value); }
        }

        protected override void OnValueChanged(RenderSettings newValue, RenderSettings oldValue, DependencyObject d) {
            base.OnValueChanged(newValue, oldValue, d);
            this.RaisePropertyChanged(nameof(this.Offset));
            this.RaisePropertyChanged(nameof(this.OffsetType));
            this.RaisePropertyChanged(nameof(this.FlipHorizontal));
            this.RaisePropertyChanged(nameof(this.FlipVertical));
        }
    }
}