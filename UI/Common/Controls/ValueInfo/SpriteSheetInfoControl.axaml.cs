namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Unity;

public partial class SpriteSheetInfoControl : ValueControl<SpriteSheet> {
    public static readonly DirectProperty<SpriteSheetInfoControl, SpriteDisplayCollection> SpriteCollectionProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetInfoControl, SpriteDisplayCollection>(
            nameof(SpriteCollection),
            control => control.SpriteCollection);

    public SpriteSheetInfoControl() : base() {
    }

    [InjectionConstructor]
    public SpriteSheetInfoControl(ValueControlDependencies dependencies) : base(dependencies) {
        if (this.Owner is ContentFile file) {
            this.SpriteCollection = SpriteDisplayCollection.Create(this.Value, file);
        }

        this.InitializeComponent();
    }

    public SpriteDisplayCollection SpriteCollection { get; }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnDetachedFromVisualTree(e);

        this.SpriteCollection?.Dispose();
    }
}