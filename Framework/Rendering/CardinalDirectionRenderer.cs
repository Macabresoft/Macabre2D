namespace Macabre2D.Framework;

using Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A renderer that combines <see cref="InputAction" /> with <see cref="GamePadIconSet" /> and <see cref="KeyboardIconSet" />
/// to render <see cref="Buttons" />,  <see cref="Keys" />, and <see cref="MouseButton" /> associated with actions.
/// </summary>
public class CardinalDirectionRenderer : IconSetRenderer<CardinalDirections, CardinalDirectionsIconSet, CardinalDirectionIconSetReference> {
}