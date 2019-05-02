namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using System.Windows;
    using System.Windows.Input;

    public partial class SpriteEditor : NamedValueEditor<Sprite> {

        public static readonly DependencyProperty SelectSpriteCommandProperty = DependencyProperty.Register(
            nameof(SelectSpriteCommand),
            typeof(ICommand),
            typeof(SpriteEditor),
            new PropertyMetadata());

        private string _absolutePathToImage;
        private SpriteWrapper _spriteWrapper;

        public SpriteEditor() {
            this.InitializeComponent();
        }

        public ICommand SelectSpriteCommand {
            get { return (ICommand)this.GetValue(SelectSpriteCommandProperty); }
            set { this.SetValue(SelectSpriteCommandProperty, value); }
        }

        public SpriteWrapper SpriteWrapper {
            get {
                return this._spriteWrapper;
            }

            set {
                this._spriteWrapper = value;
                this.Value = this._spriteWrapper?.Sprite;
                this.RaisePropertyChanged(nameof(this.SpriteWrapper));
            }
        }

        internal string AbsolutePathToImage {
            get {
                return this._absolutePathToImage;
            }
            set {
                if (value != this._absolutePathToImage) {
                    this._absolutePathToImage = value;
                    this.RaisePropertyChanged(this.AbsolutePathToImage);
                }
            }
        }

        protected override void OnValueChanged(Sprite newValue, Sprite oldValue, DependencyObject d) {
            if (d is SpriteEditor editor) {
                editor.RaisePropertyChanged(nameof(this.SpriteWrapper));
            }

            base.OnValueChanged(newValue, oldValue, d);
        }
    }
}