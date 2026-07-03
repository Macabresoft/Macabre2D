namespace Macabre2D.Framework;

using System.Collections.Generic;

/// <summary>
/// A helper for licenses used by Macabre2D, including its editor.
/// </summary>
public static class LicenseHelper {
    /// <summary>
    /// Gets all license definitions.
    /// </summary>
    public static IReadOnlyCollection<LicenseDefinition> Definitions { get; } = [
        new("Macabre2D", Licenses.Macabre2D),
        new("Macabresoft.Core", Licenses.Macabresoft_Core),
        new("MonoGame", Licenses.MonoGame),
        new("AvaloniaUI", Licenses.AvaloniaUI),
        new("Public Sans", Licenses.Public_Sans),
        new("Fluent UI System Icons (Microsoft)", Licenses.Fluent_UI_System_Icons),
        new("Newtonsoft.Json", Licenses.Newtonsoft_Json),
        new("Fluent Assertions", Licenses.AvaloniaUI),
        new("Microsoft.NET.Test.Sdk", Licenses.Microsoft_NET_Test_Sdk),
        new("NSubstitute", Licenses.NSubstitute),
        new("NUnit", Licenses.NUnit),
        new("NUnit 3 Test Adapter", Licenses.NUnit3TestAdapter),
        new("Unity", Licenses.Unity),
        new("Microsoft .NET Library", Licenses.Microsoft__NET_Library),
        new("NuGet", Licenses.NuGet),
        new("ReactiveUI", Licenses.ReactiveUI)
    ];
}