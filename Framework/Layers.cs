namespace Macabre2D.Framework {

    using System;

    /// <summary>
    /// The 16 layers to be used in bit-masking operations by rendering and the physics engine.
    /// </summary>
    [Flags]
    public enum Layers : ushort {
        None = 0,
        Default = 1 << 0,
        Player = 1 << 1,
        Enemy = 1 << 2,
        NPC = 1 << 3,
        Environment = 1 << 4,
        Background = 1 << 5,
        Foreground = 1 << 6,
        Prop = 1 << 7,
        Interactive = 1 << 8,
        Effects = 1 << 9,
        UI = 1 << 10,
        Accessibility = 1 << 11,
        Custom1 = 1 << 12,
        Custom2 = 1 << 13,
        Custom3 = 1 << 14,
        Custom4 = 1 << 15,
        All = ushort.MaxValue
    }
}