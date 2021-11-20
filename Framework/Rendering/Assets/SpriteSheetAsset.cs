namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base asset for assets which are packaged in a <see cref="SpriteSheet" />.
    /// </summary>
    [DataContract]
    public class SpriteSheetAsset : NotifyPropertyChanged, IIdentifiable, INameable {
        private string _name = string.Empty;

        /// <inheritdoc />
        [DataMember]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <inheritdoc />
        [DataMember]
        public string Name {
            get => this._name;
            set => this.Set(ref this._name, value);
        }
        
        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        public SpriteSheet? SpriteSheet { get; internal set; }
    }
}