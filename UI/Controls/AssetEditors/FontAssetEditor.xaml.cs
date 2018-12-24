namespace Macabre2D.UI.Controls.AssetEditors {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Unity;

    public partial class FontAssetEditor : UserControl, INotifyPropertyChanged {

        public static DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(FontAsset),
            typeof(FontAssetEditor),
            new PropertyMetadata(null, new PropertyChangedCallback(OnAssetChanged)));

        private readonly IUndoService _undoService;

        public FontAssetEditor() {
            this._undoService = ViewContainer.Instance.Resolve<IUndoService>();
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FontAsset Asset {
            get { return (FontAsset)this.GetValue(AssetProperty); }
            set { this.SetValue(AssetProperty, value); }
        }

        public Models.FontStyle AssetFontStyle {
            get {
                return this.Asset != null ? this.Asset.Style : Models.FontStyle.Regular;
            }

            set {
                if (this.Asset != null) {
                    var originalValue = this.Asset.Style;
                    var newValue = value;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.Asset.Style = newValue;
                            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Style)));
                        },
                        () => {
                            this.Asset.Style = originalValue;
                            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Style)));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public string FontName {
            get {
                return this.Asset?.FontName;
            }

            set {
                if (this.Asset != null) {
                    var originalValue = this.Asset.FontName;
                    var newValue = value;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.Asset.FontName = newValue;
                            this.RaisePropertyChanged(nameof(this.FontName));
                        },
                        () => {
                            this.Asset.FontName = originalValue;
                            this.RaisePropertyChanged(nameof(this.FontName));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public float Size {
            get {
                return this.Asset != null ? this.Asset.Size : 12f;
            }

            set {
                if (this.Asset != null) {
                    var originalValue = this.Asset.Size;
                    var newValue = value;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.Asset.Size = newValue;
                            this.RaisePropertyChanged(nameof(this.Size));
                        },
                        () => {
                            this.Asset.Size = originalValue;
                            this.RaisePropertyChanged(nameof(this.Size));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public float Spacing {
            get {
                return this.Asset != null ? this.Asset.Spacing : 0f;
            }

            set {
                if (this.Asset != null) {
                    var originalValue = this.Asset.Spacing;
                    var newValue = value;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.Asset.Spacing = newValue;
                            this.RaisePropertyChanged(nameof(this.Spacing));
                        },
                        () => {
                            this.Asset.Spacing = originalValue;
                            this.RaisePropertyChanged(nameof(this.Spacing));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public bool UseKerning {
            get {
                return this.Asset != null ? this.Asset.UseKerning : true;
            }

            set {
                if (this.Asset != null) {
                    var originalValue = this.Asset.UseKerning;
                    var newValue = value;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.Asset.UseKerning = newValue;
                            this.RaisePropertyChanged(nameof(this.UseKerning));
                        },
                        () => {
                            this.Asset.UseKerning = originalValue;
                            this.RaisePropertyChanged(nameof(this.UseKerning));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        private static void OnAssetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is FontAssetEditor control && e.NewValue != null) {
                control.RaisePropertyChanged(nameof(control.FontName));
                control.RaisePropertyChanged(nameof(control.Size));
                control.RaisePropertyChanged(nameof(control.Style));
                control.RaisePropertyChanged(nameof(control.Spacing));
                control.RaisePropertyChanged(nameof(control.UseKerning));
            }
        }

        private void RaisePropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}