namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An interface for a render system which exclusively renders entities that implement <see cref="ILegacyFontRenderer" />.
/// </summary>
public interface ILegacyFontRenderSystem {
    /// <summary>
    /// Called when <see cref="ShouldRenderLegacyFonts" /> changes.
    /// </summary>
    event EventHandler? ShouldRenderLegacyFontsChanged;

    /// <summary>
    /// Gets a value indicating whether this should render.
    /// </summary>
    bool ShouldRenderLegacyFonts { get; }

    /// <summary>
    /// Renders entities as defined by this system.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="renderSize">The render size.</param>
    void RenderLegacyFonts(FrameTime frameTime, Point renderSize);
}

/// <summary>
/// A render system which exclusively renders entities that implement <see cref="ILegacyFontRenderer" />.
/// </summary>
public sealed class LegacyFontRenderSystem : GameSystem, ILegacyFontRenderSystem {
    private QuadTree<ILegacyFontRenderer> _renderTree = QuadTree<ILegacyFontRenderer>.Default;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderLegacyFontsChanged;

    /// <inheritdoc />
    [DataMember]
    public bool ShouldRenderLegacyFonts {
        get;
        set {
            if (this.Set(ref field, value)) {
                this.ShouldRenderLegacyFontsChanged.SafeInvoke(this);
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
    public void RenderLegacyFonts(FrameTime frameTime, Point renderSize) {
        this.InsertRenderables();

        foreach (var camera in this.Scene.Cameras) {
            camera.RenderLegacyFonts(frameTime, this.Game.SpriteBatch, renderSize, this._renderTree);
        }
    }

    private void InsertRenderables() {
        this._renderTree.Clear();

        if (!this.Scene.BoundingArea.IsEmpty) {
            foreach (var entity in this.Scene.LegacyFontRenderers.Where(x => x.RenderOutOfBounds || x.BoundingArea.OverlapsExclusive(this.Scene.BoundingArea))) {
                this._renderTree.Insert(entity);
            }
        }
        else {
            foreach (var entity in this.Scene.LegacyFontRenderers) {
                this._renderTree.Insert(entity);
            }
        }
    }

    private void ResetTree() {
        this._renderTree.Clear();

        if (this.Scene.BoundingArea.IsEmpty) {
            this._renderTree = QuadTree<ILegacyFontRenderer>.Default;
        }
        else {
            this._renderTree = new QuadTree<ILegacyFontRenderer>(
                0,
                this.Scene.BoundingArea.Minimum.X,
                this.Scene.BoundingArea.Minimum.Y,
                this.Scene.BoundingArea.Width,
                this.Scene.BoundingArea.Height);
        }
    }

    private void Scene_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IScene.BoundingArea)) {
            this.ResetTree();
        }
    }
}