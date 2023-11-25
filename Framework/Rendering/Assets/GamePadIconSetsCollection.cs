﻿namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An observable collection of <see cref="GamePadIconSet" />.
/// </summary>
[DataContract]
[Category("Game Pad Icon Sets")]
public class GamePadIconSetsCollection : NameableCollection<GamePadIconSet> {
    /// <inheritdoc />
    public override string Name => "Game Pad Icon Sets";
}