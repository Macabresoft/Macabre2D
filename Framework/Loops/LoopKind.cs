namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// The kind of loop being run.
/// </summary>
public enum LoopKind {
    None = 0,
    Update = 1,
    Render = 2,
    Async = 3,
    PreUpdate = 4,
    PostUpdate = 5
}