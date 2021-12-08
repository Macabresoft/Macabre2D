namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;

/// <summary>
/// A helper for licenses used by Macabre2D, including its editor.
/// </summary>
public static class LicenseHelper {
    /// <summary>
    /// Gets all license definitions.
    /// </summary>
    public static IReadOnlyCollection<LicenseDefinition> Definitions { get; } = new LicenseDefinition[] {
        new("Macabre2D", Licenses.Macabre2D),
        new("Macabresoft.Core", Licenses.Macabresoft_Core),
        new("MonoGame", Licenses.MonoGame),
        new("Mono.Xna", Licenses.Mono_Xna),
        new("AvaloniaUI", Licenses.AvaloniaUI),
        new("Public Sans", Licenses.Public_Sans),
        new("Fluent UI System Icons (Microsoft)", Licenses.Fluent_UI_System_Icons),
        new("Newtonsoft.Json", Licenses.Newtonsoft_Json),
        new("Fluent Assertions", Licenses.AvaloniaUI),
        new("Microsoft.NET.Test.Sdk", Licenses.Microsoft_NET_Test_Sdk),
        new("Mono.Cecil", Licenses.Mono_Cecil),
        new("NSubstitute", Licenses.NSubstitute),
        new("NUnit", Licenses.NUnit),
        new("NUnit 3 Test Adapter", Licenses.NUnit3TestAdapter),
        new("Unity", Licenses.Unity),
        new("AssimpNet", Licenses.AssimpNet),
        new("Castle.Core", Licenses.Castle_Core),
        new("DynamicData", Licenses.DynamicData),
        new("HalfBuzzSharp", Licenses.SkiaSharp),
        new("JetBRains.Annotations", Licenses.Jetbrains_Annotations),
        new("Microsoft .NET Library", Licenses.Microsoft__NET_Library),
        new("NuGet", Licenses.NuGet),
        new("ReactiveUI", Licenses.ReactiveUI),
        new("SkiaSharp", Licenses.SkiaSharp),
        new("Splat", Licenses.Splat),
        new("System.Reactive", Licenses.System_Reactive),
        new("Tmds.DBus", Licenses.Tmds_DBus)
    };
}