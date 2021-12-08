namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

public class SpriteDisplayCollectionControl : UserControl {
    public static readonly StyledProperty<SpriteDisplayCollection> CollectionProperty =
        AvaloniaProperty.Register<SpriteDisplayCollectionControl, SpriteDisplayCollection>(
            nameof(Collection),
            defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<SpriteDisplayModel> SelectedSpriteProperty =
        AvaloniaProperty.Register<SpriteDisplayCollectionControl, SpriteDisplayModel>(
            nameof(SelectedSprite),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<ThumbnailSize> ThumbnailSizeProperty =
        AvaloniaProperty.Register<SpriteDisplayCollectionControl, ThumbnailSize>(
            nameof(ThumbnailSize),
            defaultBindingMode: BindingMode.OneWay);

    public SpriteDisplayCollectionControl() {
        this.InitializeComponent();
    }

    public SpriteDisplayCollection Collection {
        get => this.GetValue(CollectionProperty);
        set => this.SetValue(CollectionProperty, value);
    }

    public SpriteDisplayModel SelectedSprite {
        get => this.GetValue(SelectedSpriteProperty);
        set => this.SetValue(SelectedSpriteProperty, value);
    }

    public ThumbnailSize ThumbnailSize {
        get => this.GetValue(ThumbnailSizeProperty);
        set => this.SetValue(ThumbnailSizeProperty, value);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}