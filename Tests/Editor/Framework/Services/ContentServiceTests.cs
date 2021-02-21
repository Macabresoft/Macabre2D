namespace Macabresoft.Macabre2D.Tests.Editor.Framework.Services {
    using System.IO;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using NUnit.Framework;

    [TestFixture]
    public sealed class ContentServiceTests {
        private const string BinDirectoryName = "bin";
        private const string ContentDirectoryName = "Content";
        private const string ContentFileName = "Content.mgcb";
        private const string LeagueMonoXnbName = "League Mono.xnb";
        private const string PlatformName = "DesktopGL";
        private const string SkullXnbName = "skull.xnb";

        [Test]
        [Category("Integration Tests")]
        public void Build_ShouldRunMGCB() {
            var service = new ContentService(new Serializer());
            var contentDirectory = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                PathHelper.GetPathToAncestorDirectory(3),
                ContentDirectoryName);
            var contentFile = Path.Combine(contentDirectory, ContentFileName);
            var binDirectory = Path.Combine(contentDirectory, BinDirectoryName);
            var buildContentDirectory = Path.Combine(binDirectory, PlatformName);
            var skullFilePath = Path.Combine(buildContentDirectory, SkullXnbName);
            var leagueMonoFilePath = Path.Combine(buildContentDirectory, LeagueMonoXnbName);

            if (Directory.Exists(binDirectory)) {
                Directory.Delete(binDirectory, true);
            }

            using (new AssertionScope()) {
                service.Build(new BuildContentArguments(contentFile, PlatformName, false)).Should().Be(0);
                File.Exists(skullFilePath).Should().BeTrue();
                File.Exists(leagueMonoFilePath).Should().BeTrue();
            }
        }

        [Test]
        [Category("Integration Tests")]
        public void Initialize_ShouldBuildContentHierarchy() {
            
        }
    }
}