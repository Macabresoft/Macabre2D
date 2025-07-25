namespace Macabresoft.Macabre2D.UI.Tests;

using System.IO;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.UI.Common;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class BuildServiceTests {
    [Test]
    [Category("Integration Tests")]
    public void Build_ShouldRunMGCB() {
        var service = new BuildService(Substitute.For<IAssemblyService>(), new FileSystemService(), new PathService(), new ProcessService(), Substitute.For<ISerializer>());

        var contentDirectory = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            PathHelper.GetPathToAncestorDirectory(3),
            PathService.ContentDirectoryName);
        var contentFile = Path.Combine(contentDirectory, ContentFileName);
        var binDirectory = Path.Combine(contentDirectory, BinDirectoryName);
        var skullFilePath = Path.Combine(binDirectory, SkullXnbName);
        var leagueMonoFilePath = Path.Combine(binDirectory, LeagueMonoXnbName);

        if (Directory.Exists(binDirectory)) {
            Directory.Delete(binDirectory, true);
        }

        using (new AssertionScope()) {
            service.BuildContent(new BuildContentArguments(contentFile, PlatformName, false)).Should().Be(0);
            File.Exists(skullFilePath).Should().BeTrue();
            File.Exists(leagueMonoFilePath).Should().BeTrue();
        }
    }

    private const string BinDirectoryName = "bin";
    private const string ContentFileName = "Content.mgcb";
    private const string LeagueMonoXnbName = "League Mono.xnb";
    private const string PlatformName = "DesktopGL";
    private const string SkullXnbName = "skull.xnb";
}