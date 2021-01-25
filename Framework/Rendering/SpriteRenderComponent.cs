namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A component which will render a single sprite.
    /// </summary>
    [Display(Name = "Sprite Renderer")]
    public sealed class SpriteRenderComponent : BaseSpriteComponent {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderComponent" /> class.
        /// </summary>
        public SpriteRenderComponent() : base() {
            this.SpriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
        }

        /// <inheritdoc />
        public override byte SpriteIndex => this.SpriteReference.SpriteIndex;

        /// <summary>
        /// Gets the sprite reference this component will render.
        /// </summary>
        [DataMember(Order = 0)]
        [Display(Name = "Sprite")]
        public SpriteReference SpriteReference { get; } = new();

        /// <inheritdoc />
        public override SpriteSheet? SpriteSheet => this.SpriteReference.Asset;

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            AssetManager.Instance.ResolveAsset<SpriteSheet, Texture2D>(this.SpriteReference);
            this.SpriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
        }

        private void SpriteReference_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.SpriteSheet.SpriteSize)) {
                this.Reset();
            }
        }
    }
}