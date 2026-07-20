namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A container for a scene system asset.
/// </summary>
public class SceneSystemContainer : SceneSystem, IRenderSystem, ISceneUpdateSystem, IScreenSpaceRenderSystem {
    private IRenderSystem _renderSystem = EmptyObject.Instance;
    private IScreenSpaceRenderSystem _screenSpaceRenderSystem = EmptyObject.Instance;
    private ISceneUpdateSystem _updateSystem = EmptyObject.Instance;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderInScreenSpaceChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <inheritdoc />
    public SceneUpdateSystemKind Kind => this._updateSystem.Kind;

    /// <inheritdoc />
    public bool ShouldRender => this._renderSystem.ShouldRender;

    /// <inheritdoc />
    public bool ShouldRenderInScreenSpace => this._screenSpaceRenderSystem.ShouldRenderInScreenSpace;

    /// <inheritdoc />
    public bool ShouldUpdate => this._updateSystem.ShouldUpdate;

    /// <summary>
    /// Gets the system.
    /// </summary>
    public ISceneSystem? System { get; private set; }

    /// <summary>
    /// Gets the system asset reference.
    /// </summary>
    [DataMember]
    public SceneSystemAssetReference SystemReference { get; } = new();

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this._renderSystem.ShouldRenderChanged -= this.RenderSystem_ShouldRenderChanged;
        this._screenSpaceRenderSystem.ShouldRenderInScreenSpaceChanged -= this.ScreenSpaceRenderSystem_ShouldRenderInScreenSpaceChanged;
        this._updateSystem.ShouldUpdateChanged -= this.UpdateSystem_ShouldUpdateChanged;

        this._renderSystem = EmptyObject.Instance;
        this._screenSpaceRenderSystem = EmptyObject.Instance;
        this._updateSystem = EmptyObject.Instance;
        this.System = null;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);

        if (!BaseGame.IsDesignMode && this.SystemReference.TryGetSystem(out var system)) {
            this.System = system;

            if (system is IRenderSystem renderSystem) {
                this._renderSystem = renderSystem;
                this._renderSystem.ShouldRenderChanged += this.RenderSystem_ShouldRenderChanged;
            }

            if (system is IScreenSpaceRenderSystem screenSpaceRenderSystem) {
                this._screenSpaceRenderSystem = screenSpaceRenderSystem;
                this._screenSpaceRenderSystem.ShouldRenderInScreenSpaceChanged += this.ScreenSpaceRenderSystem_ShouldRenderInScreenSpaceChanged;
            }

            if (system is ISceneUpdateSystem updateSystem) {
                this._updateSystem = updateSystem;
                this._updateSystem.ShouldUpdateChanged += this.UpdateSystem_ShouldUpdateChanged;
            }

            system.Initialize(scene);
        }
    }

    /// <inheritdoc />
    public override void LoadAssets(IAssetManager assets, IGame game) {
        base.LoadAssets(assets, game);

        if (!BaseGame.IsDesignMode && this.SystemReference.TryGetSystem(out var system)) {
            system.LoadAssets(assets, game);
        }
    }

    /// <inheritdoc />
    public override void OnSceneTreeLoaded() {
        base.OnSceneTreeLoaded();

        if (!BaseGame.IsDesignMode && this.SystemReference.TryGetSystem(out var system)) {
            system.OnSceneTreeLoaded();
        }
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime) {
        this._renderSystem.Render(frameTime);
    }

    /// <inheritdoc />
    public void RenderInScreenSpace(FrameTime frameTime) {
        this._screenSpaceRenderSystem.RenderInScreenSpace(frameTime);
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        this._updateSystem.Update(frameTime, inputState);
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.SystemReference;
    }

    private void RenderSystem_ShouldRenderChanged(object? sender, EventArgs e) {
        this.ShouldRenderChanged.SafeInvoke(this);
    }

    private void ScreenSpaceRenderSystem_ShouldRenderInScreenSpaceChanged(object? sender, EventArgs e) {
        this.ShouldRenderInScreenSpaceChanged.SafeInvoke(this);
    }

    private void UpdateSystem_ShouldUpdateChanged(object? sender, EventArgs e) {
        this.ShouldRenderChanged.SafeInvoke(this);
    }
}