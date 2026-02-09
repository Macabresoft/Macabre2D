namespace Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A base asset for assets which are packaged in a <see cref="SpriteSheet" />.
/// </summary>
[DataContract]
public abstract class SpriteSheetMember : PropertyChangedNotifier, IIdentifiable, INameable {
    /// <inheritdoc />
    [DataMember]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets the initial sprite index.
    /// </summary>
    public virtual byte? InitialSpriteIndex => null;

    /// <inheritdoc />
    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    public SpriteSheet? SpriteSheet { get; internal set; }
}