namespace Macabresoft.Macabre2D.Framework;

using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// A <see cref="RenderPriority" /> wrapper that can be enabled or disabled.
/// </summary>
public class RenderPriorityOverride : ValueOverride<RenderPriority> {
    /// <summary>
    /// Initializes a new instance of <see cref="RenderPriorityOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isEnabled">A value indicating whether this is enabled.</param>
    public RenderPriorityOverride(RenderPriority value, bool isEnabled) : base(value, isEnabled) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RenderPriorityOverride" />
    /// </summary>
    /// <param name="value">The value.</param>
    public RenderPriorityOverride(RenderPriority value) : this(value, false) {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RenderPriorityOverride" />
    /// </summary>
    public RenderPriorityOverride() : this(default) {
    }
}