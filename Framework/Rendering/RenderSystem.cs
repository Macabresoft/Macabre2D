namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for a <see cref="IGameSystem" /> that has an update loop.
/// </summary>
public interface IRenderSystem : IGameSystem {
    /// <summary>
    /// Called when <see cref="ShouldRender" /> changes.
    /// </summary>
    event EventHandler? ShouldRenderChanged;

    /// <summary>
    /// Gets a value indicating whether this should render.
    /// </summary>
    bool ShouldRender { get; }

    /// <summary>
    /// Renders entities as defined by this system.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    void Render(FrameTime frameTime);
}

/// <summary>
/// The default rendering system, which attempts to render every
/// <see cref="IRenderableEntity" /> in the scene.
/// </summary>
public class RenderSystem : GameSystem, IRenderSystem, IScreenSpaceRenderSystem {
    private QuadTree<IScreenSpaceRenderer> _legacyRenderTree = QuadTree<IScreenSpaceRenderer>.Default;
    private QuadTree<IRenderableEntity> _renderTree = QuadTree<IRenderableEntity>.Default;


    /// <inheritdoc />
    public event EventHandler? ShouldRenderChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderInScreenSpaceChanged;

    /// <inheritdoc />
    [DataMember]
    public bool ShouldRender {
        get;
        set {
            if (this.Set(ref field, value)) {
                this.ShouldRenderChanged.SafeInvoke(this);
            }
        }
    } = true;

    /// <inheritdoc />
    [DataMember]
    public bool ShouldRenderInScreenSpace {
        get;
        set {
            if (this.Set(ref field, value)) {
                this.ShouldRenderInScreenSpaceChanged.SafeInvoke(this);
            }
        }
    } = true;


    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.Scene.PropertyChanged -= this.Scene_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);

        if (!Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.Scene.PropertyChanged += this.Scene_PropertyChanged;
        }

        this.ResetTree();
    }

    /// <inheritdoc />
    public virtual void Render(FrameTime frameTime) {
        this.InsertRenderables();

        foreach (var camera in this.Scene.Cameras) {
            camera.Render(frameTime, this.Game.SpriteBatch, this._renderTree);
        }
    }

    /// <inheritdoc />
    public void RenderInScreenSpace(FrameTime frameTime) {
        this.InsertLegacyRenderables();

        foreach (var camera in this.Scene.Cameras) {
            camera.RenderInScreenSpace(frameTime, this.Game.SpriteBatch, this._legacyRenderTree);
        }
    }

    private void InsertLegacyRenderables() {
        this._legacyRenderTree.Clear();

        if (!this.Scene.BoundingArea.IsEmpty) {
            foreach (var entity in this.Scene.LegacyFontRenderers.Where(x => x.RenderOutOfBounds || x.BoundingArea.OverlapsExclusive(this.Scene.BoundingArea))) {
                this._legacyRenderTree.Insert(entity);
            }
        }
        else {
            foreach (var entity in this.Scene.LegacyFontRenderers) {
                this._legacyRenderTree.Insert(entity);
            }
        }
    }

    private void InsertRenderables() {
        this._renderTree.Clear();

        if (!this.Scene.BoundingArea.IsEmpty) {
            foreach (var entity in this.Scene.RenderableEntities.Where(x => x.RenderOutOfBounds || x.BoundingArea.OverlapsExclusive(this.Scene.BoundingArea))) {
                this._renderTree.Insert(entity);
            }
        }
        else {
            foreach (var entity in this.Scene.RenderableEntities) {
                this._renderTree.Insert(entity);
            }
        }
    }

    private void ResetTree() {
        this._renderTree.Clear();
        this._legacyRenderTree.Clear();

        if (this.Scene.BoundingArea.IsEmpty) {
            this._renderTree = QuadTree<IRenderableEntity>.Default;
        }
        else {
            this._renderTree = new QuadTree<IRenderableEntity>(
                0,
                this.Scene.BoundingArea.Minimum.X,
                this.Scene.BoundingArea.Minimum.Y,
                this.Scene.BoundingArea.Width,
                this.Scene.BoundingArea.Height);
        }

        if (this.ShouldRenderInScreenSpace) {
            if (this.Scene.BoundingArea.IsEmpty) {
                this._legacyRenderTree = QuadTree<IScreenSpaceRenderer>.Default;
            }
            else {
                this._legacyRenderTree = new QuadTree<IScreenSpaceRenderer>(
                    0,
                    this.Scene.BoundingArea.Minimum.X,
                    this.Scene.BoundingArea.Minimum.Y,
                    this.Scene.BoundingArea.Width,
                    this.Scene.BoundingArea.Height);
            }
        }
    }

    private void Scene_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IScene.BoundingArea)) {
            this.ResetTree();
        }
    }
}