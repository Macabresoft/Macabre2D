namespace Macabre2D.Framework;

using System;

/// <summary>
/// Defines a field or property as a resource name for the Macabre2D editor.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ResourceNameAttribute : Attribute {
    
}