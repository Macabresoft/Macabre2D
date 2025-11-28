namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// Renders a mouse cursor.
/// </summary>
public class MouseCursorRenderer : BaseSpriteEntity, IUpdateableEntity {
    private MouseCursorAppearance _appearance = MouseCursorAppearance.Standard;
    private bool _shouldUpdate = true;
    private byte? _spriteIndex;
    private SpriteSheet? _spriteSheet;
    private int _updateOrder;

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

    /// <summary>
    /// Gets or sets the appearance of this mouse cursor.
    /// </summary>
    public MouseCursorAppearance Appearance {
        get => this._appearance;
        set {
            if (this._appearance != value) {
                this._appearance = value;
                this.ResetSpriteIndex();
            }
        }
    }

    /// <summary>
    /// Gets a reference to the camera to which this mouse cursor is related.
    /// </summary>
    [DataMember(Order = 10)]
    public EntityReference<ICamera> CameraReference { get; } = new();

    /// <summary>
    /// Gets a reference to the mouse cursor icon set.
    /// </summary>
    [DataMember(Order = 10)]
    public MouseCursorIconSetReference MouseCursorIconSet { get; } = new();

    /// <inheritdoc />
    [DataMember]
    public bool ShouldUpdate {
        get => this._shouldUpdate && this.IsEnabled;
        set {
            if (this.Set(ref this._shouldUpdate, value)) {
                this.ShouldUpdateChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    public override byte? SpriteIndex => this._spriteIndex;

    /// <inheritdoc />
    [DataMember]
    [PredefinedInteger(PredefinedIntegerKind.UpdateOrder)]
    public int UpdateOrder {
        get => this._updateOrder;
        set {
            if (this.Set(ref this._updateOrder, value)) {
                this.UpdateOrderChanged.SafeInvoke(this);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this is hovering over an activatable element.
    /// </summary>
    protected bool IsHoveringOverActivatableElement { get; set; }

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this._spriteSheet;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.MouseCursorIconSet.AssetChanged -= this.MouseCursorIconSet_AssetChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.MouseCursorIconSet.AssetChanged += this.MouseCursorIconSet_AssetChanged;
        this.ResetSpriteSheet();
        this.ResetSpriteIndex();
    }

    /// <inheritdoc />
    public virtual void Update(FrameTime frameTime, InputState inputState) {
        var camera = this.GetCamera();
        this.SetWorldPosition(camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position));

        if (inputState.IsMouseButtonHeld(MouseButton.Left)) {
            this.Appearance = MouseCursorAppearance.LeftClickHeld;
        }
        else if (this.IsHoveringOverActivatableElement) {
            this.Appearance = MouseCursorAppearance.Activatable;
        }
        else {
            this.Appearance = MouseCursorAppearance.Standard;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.MouseCursorIconSet;
    }

    /// <summary>
    /// Gets the camera to which this mouse cursor is related.
    /// </summary>
    /// <returns>The camera.</returns>
    protected ICamera GetCamera() => this.CameraReference.Entity ?? EmptyObject.Instance;

    /// <inheritdoc />
    protected override IEnumerable<IGameObjectReference> GetGameObjectReferences() {
        yield return this.CameraReference;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEnableable.IsEnabled) && this._shouldUpdate) {
            this.RaisePropertyChanged(nameof(this.ShouldUpdate));
        }
    }

    private MouseCursorIconSet? GetMouseCursorIconSet() => this.GetMouseCursorReference().PackagedAsset;

    private MouseCursorIconSetReference GetMouseCursorReference() => this.MouseCursorIconSet.PackagedAsset != null ? this.MouseCursorIconSet : this.Project.Fallbacks.MouseCursorReference;

    private void MouseCursorIconSet_AssetChanged(object? sender, bool e) {
        this.ResetSpriteSheet();
        this.ResetSpriteIndex();
    }

    private void ResetSpriteIndex() {
        if (this.GetMouseCursorIconSet()?.TryGetSpriteIndex(this._appearance, out var index) != true) {
            index = null;
        }

        this._spriteIndex = index;
    }

    private void ResetSpriteSheet() {
        this._spriteSheet = this.GetMouseCursorReference().Asset;
    }
}