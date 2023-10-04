namespace Macabresoft.Macabre2D.Framework; 

/// <summary>
/// A four way margin to be used in docking.
/// </summary>
public struct DockingMargin {

    /// <summary>
    /// Initializes a new instance of the <see cref="DockingMargin" /> class.
    /// </summary>
    public DockingMargin() : this(0f) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockingMargin" /> class.
    /// </summary>
    /// <param name="margin">The margin on all sides.</param>
    public DockingMargin(float margin) : this(margin, margin) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockingMargin" /> class.
    /// </summary>
    /// <param name="horizontal">The margin on the left and right.</param>
    /// <param name="vertical">The margin on the top and bottom.</param>
    public DockingMargin(float horizontal, float vertical) : this(horizontal, vertical, horizontal, vertical) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockingMargin" /> class.
    /// </summary>
    /// <param name="left">The margin on the left.</param>
    /// <param name="top">The margin on the top.</param>
    /// <param name="right">The margin on the right.</param>
    /// <param name="bottom">The margin on the bottom.</param>
    public DockingMargin(float left, float top, float right, float bottom) {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
    }
    
    /// <summary>
    /// The top margin.
    /// </summary>
    public float Top;
    
    /// <summary>
    /// The bottom margin.
    /// </summary>
    public float Bottom;
    
    /// <summary>
    /// The left margin.
    /// </summary>
    public float Left;
    
    /// <summary>
    /// The right margin.
    /// </summary>
    public float Right;
}