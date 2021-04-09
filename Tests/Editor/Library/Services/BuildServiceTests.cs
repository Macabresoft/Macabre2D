namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using System.IO;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using NUnit.Framework;

    [TestFixture]
    public class BuildServiceTests {
        [Test]
        [Category("Integration Tests")]
        public void Build_ShouldRunMGCB() {
            var service = new BuildService(new FileSystemService(), new ProcessService());

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
}