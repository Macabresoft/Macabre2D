namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;

/// <summary>
/// A set of icons corresponding to <see cref="CardinalDirections" />.
/// </summary>
public sealed class CardinalDirectionsIconSet : SpriteSheetIconSet<CardinalDirections> {
    private const string DefaultName = "Cardinal Direction Icons";

    /// <summary>
    /// Initializes a new instance of the <see cref="CardinalDirectionsIconSet" /> class.
    /// </summary>
    public CardinalDirectionsIconSet() : base() {
        this.Name = DefaultName;
    }

    /// <inheritdoc />
    protected override void RequestIconRefresh() {
        var keys = Enum.GetValues<CardinalDirections>().ToList();
        keys.Remove(CardinalDirections.None);
        keys.Remove(CardinalDirections.All);

        foreach (var key in keys) {
            this.RefreshIcon(key);
        }
    }
}