namespace Macabresoft.Macabre2D.Framework {

    using System;

    /// <summary>
    /// The 16 layers to be used in bit-masking operations by rendering and the physics engine.
    /// </summary>
    [Flags]
    public enum Layers : ushort {
        None = 0,
        Default = 1 << 0,
        Layer01 = 1 << 1,
        Layer02 = 1 << 2,
        Layer03 = 1 << 3,
        Layer04 = 1 << 4,
        Layer05 = 1 << 5,
        Layer06 = 1 << 6,
        Layer07 = 1 << 7,
        Layer08 = 1 << 8,
        Layer09 = 1 << 9,
        Layer10 = 1 << 10,
        Layer11 = 1 << 11,
        Layer12 = 1 << 12,
        Layer13 = 1 << 13,
        Layer14 = 1 << 14,
        Layer15 = 1 << 15
    }
}