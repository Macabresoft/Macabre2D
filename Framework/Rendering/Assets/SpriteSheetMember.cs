namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Common.Attributes;

/// <summary>
/// A base asset for assets which are packaged in a <see cref="SpriteSheet" />.
/// </summary>
[DataContract]
public abstract class SpriteSheetMember : PropertyChangedNotifier, IIdentifiable, INameable {
    /// <inheritdoc />
    [DataMember]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    public SpriteSheet? SpriteSheet { get; internal set; }
}