namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component that contains a <see cref="Prefab"/>.
    /// </summary>
    /// <seealso cref="BaseComponent"/>
    /// <seealso cref="IDrawableComponent"/>
    /// <seealso cref="IAssetComponent{Sprite}"/>
    public sealed class PrefabComponent : BaseComponent, IDrawableComponent, IAssetComponent<Prefab> {
        private Prefab _prefab;

        public PrefabComponent() {
        }

        /// <inheritdoc/>
        public BoundingArea BoundingArea {
            get {
                return this.Prefrab?.Component is IBoundable boundable ? boundable.BoundingArea : new BoundingArea();
            }
        }

        /// <summary>
        /// Gets or sets the prefrab.
        /// </summary>
        /// <value>The prefrab.</value>
        [DataMember]
        public Prefab Prefrab {
            get {
                return this._prefab;
            }

            set {
                if (this._prefab != value) {
                    this._prefab = value;

                    if (this._prefab?.Component != null && this.IsInitialized) {
                        this._prefab.Component.Initialize(this.Scene);
                    }

                    this.LoadContent();
                }
            }
        }

        /// <inheritdoc/>
        public void Draw(FrameTime frameTime, BoundingArea viewBoundingArea) {
            var component = this.Prefrab?.Component;
            if (component != null) {
                var componentWorldTransform = component.WorldTransform;
                var drawables = component.GetChildrenOfType<IDrawableComponent>();

                try {
                    component.SetWorldTransform(this.WorldTransform.Position, this.WorldTransform.Scale);
                    this.DrawComponent(component as IDrawableComponent, frameTime, viewBoundingArea);

                    foreach (var drawable in drawables) {
                        this.DrawComponent(drawable, frameTime, viewBoundingArea);
                    }
                }
                finally {
                    component.SetWorldTransform(componentWorldTransform.Position, componentWorldTransform.Scale);
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Guid> GetOwnedAssetIds() {
            return this.Prefrab != null ? new[] { this.Prefrab.Id } : new Guid[0];
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this.Prefrab?.Id == id;
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this.Scene.IsInitialized && MacabreGame.Instance.IsDesignMode) {
                this.Prefrab?.Component?.LoadContent();
            }

            base.LoadContent();
        }

        /// <inheritdoc/>
        public void RefreshAsset(Prefab newInstance) {
            if (newInstance != null && this.Prefrab?.Id == newInstance.Id) {
                this.Prefrab = newInstance;
            }
        }

        /// <inheritdoc/>
        public bool RemoveAsset(Guid id) {
            var result = this.HasAsset(id);
            if (result) {
                this.Prefrab = null;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool TryGetAsset(Guid id, out Prefab asset) {
            var result = this.Prefrab?.Id == id;
            asset = result ? this.Prefrab : null;
            return result;
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            if (this.Prefrab?.Component is BaseComponent component) {
                if (MacabreGame.Instance.IsDesignMode) {
                    if (this.Parent != null) {
                        var clone = component.Clone();
                        this.Parent.AddChild(clone);
                        this.Scene.DestroyComponent(this);
                    }
                }
                else {
                    component.Initialize(this.Scene);
                }
            }
        }

        private void DrawComponent(IDrawableComponent drawable, FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (drawable?.IsVisible == true) {
                drawable.Draw(frameTime, viewBoundingArea);
            }
        }
    }
}