namespace Macabresoft.Macabre2D.Tests.Framework.Rendering;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class PixelSnappableTests {
    [Category("Unit Tests")]
    [Test]
    public static void ShouldNotSnapToPixels_WithInherits_AndSettingsSetToFalse() {
        var project = Substitute.For<IGameProject>();
        project.SnapToPixels.Returns(false);
        var entity = new TestPixelSnappable(PixelSnap.Inherit);

        using (new AssertionScope()) {
            entity.ShouldSnapToPixels(project).Should().BeFalse();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void ShouldNotSnapToPixels_WithNo() {
        var project = Substitute.For<IGameProject>();
        var entity = new TestPixelSnappable(PixelSnap.No);

        using (new AssertionScope()) {
            project.SnapToPixels.Returns(true);
            entity.ShouldSnapToPixels(project).Should().BeFalse();
            project.SnapToPixels.Returns(false);
            entity.ShouldSnapToPixels(project).Should().BeFalse();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void ShouldSnapToPixels_WithInherits_AndSettingsSetToTrue() {
        var project = Substitute.For<IGameProject>();
        project.SnapToPixels.Returns(true);
        var entity = new TestPixelSnappable(PixelSnap.Inherit);

        using (new AssertionScope()) {
            entity.ShouldSnapToPixels(project).Should().BeTrue();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void ShouldSnapToPixels_WithYes() {
        var project = Substitute.For<IGameProject>();
        var entity = new TestPixelSnappable(PixelSnap.Yes);

        using (new AssertionScope()) {
            project.SnapToPixels.Returns(true);
            entity.ShouldSnapToPixels(project).Should().BeTrue();
            project.SnapToPixels.Returns(false);
            entity.ShouldSnapToPixels(project).Should().BeTrue();
        }
    }

    private class TestPixelSnappable : IPixelSnappable {
        public TestPixelSnappable(PixelSnap pixelSnap) {
            this.PixelSnap = pixelSnap;
        }

        public PixelSnap PixelSnap { get; }
    }
}