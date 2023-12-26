namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// Determines how game pad icons will be displayed.
/// </summary>
public enum GamePadDisplay {
    /// <summary>
    /// A controller whose face buttons are arranged:
    /// --Y--
    /// X---B
    /// --A--
    /// </summary>
    X,

    /// <summary>
    /// A controller whose face buttons are arranged:
    /// --X--
    /// Y---A
    /// --B--
    /// </summary>
    N,

    /// <summary>
    /// A controller whose face buttons are arranged:
    /// --△--
    /// □---○
    /// --X--
    /// </summary>
    S
}