namespace Macabresoft.Macabre2D.Framework {

    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The 16 layers to be used in bit-masking operations by rendering and the physics engine.
    /// </summary>
    [Flags]
    public enum Layers : ushort {
        None = 0,
        
        [Display(Name = "Default")]
        Default = 1 << 0,
        
        [Display(Name = "Layer 1")]
        Layer01 = 1 << 1,
        
        [Display(Name = "Layer 2")]
        Layer02 = 1 << 2,
        
        [Display(Name = "Layer 3")]
        Layer03 = 1 << 3,
        
        [Display(Name = "Layer 4")]
        Layer04 = 1 << 4,
        
        [Display(Name = "Layer 5")]
        Layer05 = 1 << 5,
        
        [Display(Name = "Layer 6")]
        Layer06 = 1 << 6,
        
        [Display(Name = "Layer 7")]
        Layer07 = 1 << 7,
        
        [Display(Name = "Layer 8")]
        Layer08 = 1 << 8,
        
        [Display(Name = "Layer 9")]
        Layer09 = 1 << 9,
        
        [Display(Name = "Layer 10")]
        Layer10 = 1 << 10,
        
        [Display(Name = "Layer 11")]
        Layer11 = 1 << 11,
        
        [Display(Name = "Layer 12")]
        Layer12 = 1 << 12,
        
        [Display(Name = "Layer 13")]
        Layer13 = 1 << 13,
        
        [Display(Name = "Layer 14")]
        Layer14 = 1 << 14,
        
        [Display(Name = "Layer 15")]
        Layer15 = 1 << 15
    }
}