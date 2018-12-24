namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// The 32 layers to be used in bit-masking operations by rendering and the physics engine.
    /// </summary>
    [Flags]
    public enum Layers {
        None = 0,
        Layer01 = 1 << 0,
        Layer02 = 1 << 1,
        Layer03 = 1 << 2,
        Layer04 = 1 << 3,
        Layer05 = 1 << 4,
        Layer06 = 1 << 5,
        Layer07 = 1 << 6,
        Layer08 = 1 << 7,
        Layer09 = 1 << 8,
        Layer10 = 1 << 9,
        Layer11 = 1 << 10,
        Layer12 = 1 << 11,
        Layer13 = 1 << 12,
        Layer14 = 1 << 13,
        Layer15 = 1 << 14,
        Layer16 = 1 << 15,
        Layer17 = 1 << 16,
        Layer18 = 1 << 17,
        Layer19 = 1 << 18,
        Layer20 = 1 << 19,
        Layer21 = 1 << 20,
        Layer22 = 1 << 21,
        Layer23 = 1 << 22,
        Layer24 = 1 << 23,
        Layer25 = 1 << 24,
        Layer26 = 1 << 25,
        Layer27 = 1 << 26,
        Layer28 = 1 << 27,
        Layer29 = 1 << 28,
        Layer30 = 1 << 29,
        Layer31 = 1 << 30,
        All = ~0
    }
}