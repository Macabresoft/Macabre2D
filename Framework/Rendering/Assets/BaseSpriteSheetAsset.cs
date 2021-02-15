namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A base asset for assets which are packaged in a <see cref="Package" />.
    /// </summary>
    [DataContract]
    public class SpriteSheetAsset : NotifyPropertyChanged, IIdentifiable {
        /// <inheritdoc />
        [DataMember]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}