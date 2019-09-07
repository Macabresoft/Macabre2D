namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// The 8 layers to be used in bit-masking operations by rendering and the physics engine.
    /// </summary>
    [Flags]
    public enum Layers : byte {
        None = 0,
        Layer01 = 1 << 0,
        Layer02 = 1 << 1,
        Layer03 = 1 << 2,
        Layer04 = 1 << 3,
        Layer05 = 1 << 4,
        Layer06 = 1 << 5,
        Layer07 = 1 << 6,
        Layer08 = 1 << 7,
        All = byte.MaxValue
    }
}