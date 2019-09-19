namespace Macabre2D.UI.Controls.AssetEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework.Content.Pipeline.Processors;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Unity;

    public partial class FontAssetEditor : INotifyPropertyChanged {

        public static DependencyProperty AssetProperty = DependencyProperty.Register(
            nameof(Asset),
            typeof(FontAsset),
            typeof(FontAssetEditor),
            new PropertyMetadata(null, new PropertyChangedCallback(OnAssetChanged)));

        private readonly IUndoService _undoService;

        public FontAssetEditor() {
            this._undoService = ViewContainer.Instance.Resolve<IUndoService>();
            this.SelectFontCommand = new RelayCommand(this.SelectFont);
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
                        },
                        () => {
                            this.Asset.Style = originalValue;
                        },
                        () => {
                            this.RaisePropertyChanged(nameof(this.AssetFontStyle));
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
                        },
                        () => {
                            this.Asset.FontName = originalValue;
                        },
                        () => {
                            this.RaisePropertyChanged(nameof(this.FontName));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public bool PremultiplyAlpha {
            get {
                return this.Asset != null ? this.Asset.PremultiplyAlpha : true;
            }

            set {
                if (this.Asset != null) {
                    var originalValue = this.Asset.PremultiplyAlpha;
                    var newValue = value;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.Asset.PremultiplyAlpha = newValue;
                        },
                        () => {
                            this.Asset.PremultiplyAlpha = originalValue;
                        },
                        () => {
                            this.RaisePropertyChanged(nameof(this.PremultiplyAlpha));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public ICommand SelectFontCommand { get; }

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
                        },
                        () => {
                            this.Asset.Size = originalValue;
                        },
                        () => {
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
                    var undoCommand = new UndoCommand(() => {
                        this.Asset.Spacing = newValue;
                    },
                        () => {
                            this.Asset.Spacing = originalValue;
                        },
                        () => {
                            this.RaisePropertyChanged(nameof(this.Spacing));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        public TextureProcessorOutputFormat TextureFormat {
            get {
                return this.Asset != null ? this.Asset.TextureFormat : TextureProcessorOutputFormat.Compressed;
            }

            set {
                if (this.Asset != null) {
                    var originalValue = this.Asset.TextureFormat;
                    var newValue = value;
                    var undoCommand = new UndoCommand(
                        () => {
                            this.Asset.TextureFormat = newValue;
                        },
                        () => {
                            this.Asset.TextureFormat = originalValue;
                        },
                        () => {
                            this.RaisePropertyChanged(nameof(this.TextureFormat));
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
                        },
                        () => {
                            this.Asset.UseKerning = originalValue;
                        },
                        () => {
                            this.RaisePropertyChanged(nameof(this.UseKerning));
                        });

                    this._undoService.Do(undoCommand);
                }
            }
        }

        private static void OnAssetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is FontAssetEditor control && e.NewValue != null) {
                control.RaisePropertyChanged(nameof(control.FontName));
                control.RaisePropertyChanged(nameof(control.PremultiplyAlpha));
                control.RaisePropertyChanged(nameof(control.Size));
                control.RaisePropertyChanged(nameof(control.Style));
                control.RaisePropertyChanged(nameof(control.Spacing));
                control.RaisePropertyChanged(nameof(control.TextureFormat));
                control.RaisePropertyChanged(nameof(control.UseKerning));
            }
        }

        private Models.FontStyle ConvertFontStyle(System.Drawing.FontStyle fontStyle) {
            var result = Models.FontStyle.Regular;

            if (fontStyle == System.Drawing.FontStyle.Bold) {
                result = Models.FontStyle.Bold;
            }
            else if (fontStyle == System.Drawing.FontStyle.Italic) {
                result = Models.FontStyle.Italic;
            }

            // TODO: support other styles
            return result;
        }

        private void RaisePropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SelectFont() {
            using (var fontDialog = new FontDialog()) {
                var result = fontDialog.ShowDialog();

                if (result == DialogResult.OK && fontDialog.Font != null) {
                    var name = this.FontName;
                    var size = this.Size;
                    var style = this.AssetFontStyle;

                    var undoCommand = new UndoCommand(() => {
                        this.FontName = fontDialog.Font.FontFamily.Name;
                        this.Size = fontDialog.Font.Size;
                        this.AssetFontStyle = this.ConvertFontStyle(fontDialog.Font.Style);
                    }, () => {
                        this.FontName = name;
                        this.Size = size;
                        this.AssetFontStyle = style;
                    });

                    this._undoService.Do(undoCommand);
                }
            }
        }
    }
}