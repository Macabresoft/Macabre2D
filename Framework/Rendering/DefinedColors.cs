namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using Microsoft.Xna.Framework;

/// <summary>
/// Important colors to the Macabresoft brand, represented in MonoGame's <see cref="Color" /> class.
/// </summary>
public static class DefinedColors {
    /// <summary>
    /// A cosmic teal color.
    /// </summary>
    public static Color CosmicJam = new(53, 108, 119);

    /// <summary>
    /// A not-quite-black color with a hint of <see cref="MacabresoftPurple" />.
    /// </summary>
    public static Color MacabresoftBlack = new(30, 15, 15);

    /// <summary>
    /// A not-quite-white color as seen in Macabresoft's skull logo.
    /// </summary>
    public static Color MacabresoftBone = new(227, 218, 201);

    /// <summary>
    /// The purple seen in Macabresoft's logo.
    /// </summary>
    public static Color MacabresoftPurple = new(99, 73, 96);

    /// <summary>
    /// A red that inspires revolution.
    /// </summary>
    public static Color MacabresoftRed = new(130, 38, 38);

    /// <summary>
    /// The yellow seen in Macabresoft's logo.
    /// </summary>
    public static Color MacabresoftYellow = new(200, 171, 55);

    /// <summary>
    /// A terminal-like green used in the tuner made by Macabresoft. The tuner was originally
    /// named "Zvukosti Tuner".
    /// </summary>
    public static Color ZvukostiGreen = new(113, 237, 100);

    /// <summary>
    /// Gets all the colors.
    /// </summary>
    public static readonly IReadOnlyCollection<Color> AllColors = new[] {
        MacabresoftBone,
        MacabresoftBlack,
        MacabresoftPurple,
        CosmicJam,
        MacabresoftRed,
        MacabresoftYellow,
        ZvukostiGreen
    };
}