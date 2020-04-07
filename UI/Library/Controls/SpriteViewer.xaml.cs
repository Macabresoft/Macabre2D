namespace Macabre2D.UI.Library.Controls {

    using Macabre2D.UI.Library.Models.FrameworkWrappers;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public partial class SpriteViewer : UserControl {

        public static readonly DependencyProperty ShowSpriteInfoProperty = DependencyProperty.Register(
            nameof(ShowSpriteInfo),
            typeof(bool),
            typeof(SpriteViewer),
            new PropertyMetadata(true));

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            nameof(Size),
            typeof(int),
            typeof(SpriteViewer),
            new PropertyMetadata(128));

        public static readonly DependencyProperty SpriteProperty = DependencyProperty.Register(
            nameof(Sprite),
            typeof(SpriteWrapper),
            typeof(SpriteViewer),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSpriteChanged)));

        public SpriteViewer() {
            this.InitializeComponent();
        }

        public bool ShowSpriteInfo {
            get { return (bool)this.GetValue(ShowSpriteInfoProperty); }
            set { this.SetValue(ShowSpriteInfoProperty, value); }
        }

        public int Size {
            get { return (int)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        public SpriteWrapper Sprite {
            get { return (SpriteWrapper)this.GetValue(SpriteProperty); }
            set { this.SetValue(SpriteProperty, value); }
        }

        private static void OnSpriteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is SpriteViewer editor) {
                if (e.NewValue is SpriteWrapper newValue) {
                    newValue.PropertyChanged += editor.Sprite_PropertyChanged;
                }

                if (e.OldValue is SpriteWrapper oldValue) {
                    oldValue.PropertyChanged -= editor.Sprite_PropertyChanged;
                }
            }
        }

        private void Sprite_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this._image.GetBindingExpression(Image.SourceProperty).UpdateTarget();
        }
    }
}