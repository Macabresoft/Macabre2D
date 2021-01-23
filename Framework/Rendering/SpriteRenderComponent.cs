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
        [DataMember(Order = 0)]
        [Display(Name = "Sprite")]
        private readonly SpriteReference _spriteReference = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderComponent" /> class.
        /// </summary>
        public SpriteRenderComponent() : base() {
            this._spriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
        }

        /// <inheritdoc />
        protected override byte SpriteIndex => this._spriteReference.SpriteIndex;

        /// <inheritdoc />
        protected override SpriteSheet? SpriteSheet => this._spriteReference.Asset;

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            AssetManager.Instance.ResolveAsset<SpriteSheet, Texture2D>(this._spriteReference);
            this._spriteReference.PropertyChanged += this.SpriteReference_PropertyChanged;
        }

        private void SpriteReference_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.SpriteSheet.SpriteSize)) {
                this.Reset();
            }
        }
    }
}