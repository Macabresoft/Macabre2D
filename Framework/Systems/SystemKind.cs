namespace Macabresoft.Macabre2D.Framework; 

/// <summary>
/// Defines the loop that a system should run in.
/// </summary>
public enum SystemKind {
    None = 0,
    Update = 1,
    Render = 2,
    Async = 3
}