namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueInfo {
    using System;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media.Imaging;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Models.Rendering;

    public class SpriteSheetInfoControl : ValueControl<SpriteSheet> {
        public static readonly DirectProperty<SpriteSheetInfoControl, Bitmap> BitmapProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetInfoControl, Bitmap>(
                nameof(Bitmap),
                control => control.Bitmap);

        public static readonly DirectProperty<SpriteSheetInfoControl, SpriteDisplayCollection> SpriteCollectionProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetInfoControl, SpriteDisplayCollection>(
                nameof(SpriteCollection),
                control => control.SpriteCollection);

        private Bitmap _bitmap;

        private SpriteDisplayCollection _spriteCollection;

        public SpriteSheetInfoControl() {
            this.InitializeComponent();
        }

        public Bitmap Bitmap {
            get => this._bitmap;
            private set => this.SetAndRaise(BitmapProperty, ref this._bitmap, value);
        }

        public SpriteDisplayCollection SpriteCollection {
            get => this._spriteCollection;
            private set => this.SetAndRaise(SpriteCollectionProperty, ref this._spriteCollection, value);
        }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);

            if (owner is ContentFile file) {
                this.SpriteCollection = new SpriteDisplayCollection(this.Value, file);
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnDetachedFromVisualTree(e);

            this._spriteCollection?.Dispose();
            this._bitmap?.Dispose();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}