namespace Macabresoft.Macabre2D.Tests.Framework.Rendering;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class PixelSnappableTests {
    private class TestPixelSnappable : IPixelSnappable {
        public TestPixelSnappable(PixelSnap pixelSnap) {
            this.PixelSnap = pixelSnap;
        }

        public PixelSnap PixelSnap { get; }
    }

    [Category("Unit Tests")]
    [Test]
    public static void ShouldNotSnapToPixels_WithInherits_AndSettingsSetToFalse() {
        var settings = Substitute.For<IGameSettings>();
        settings.SnapToPixels.Returns(false);
        var entity = new TestPixelSnappable(PixelSnap.Inherit);

        using (new AssertionScope()) {
            entity.ShouldSnapToPixels(settings).Should().BeFalse();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void ShouldNotSnapToPixels_WithNo() {
        var settings = Substitute.For<IGameSettings>();
        var entity = new TestPixelSnappable(PixelSnap.No);

        using (new AssertionScope()) {
            settings.SnapToPixels.Returns(true);
            entity.ShouldSnapToPixels(settings).Should().BeFalse();
            settings.SnapToPixels.Returns(false);
            entity.ShouldSnapToPixels(settings).Should().BeFalse();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void ShouldSnapToPixels_WithInherits_AndSettingsSetToTrue() {
        var settings = Substitute.For<IGameSettings>();
        settings.SnapToPixels.Returns(true);
        var entity = new TestPixelSnappable(PixelSnap.Inherit);

        using (new AssertionScope()) {
            entity.ShouldSnapToPixels(settings).Should().BeTrue();
        }
    }

    [Category("Unit Tests")]
    [Test]
    public static void ShouldSnapToPixels_WithYes() {
        var settings = Substitute.For<IGameSettings>();
        var entity = new TestPixelSnappable(PixelSnap.Yes);

        using (new AssertionScope()) {
            settings.SnapToPixels.Returns(true);
            entity.ShouldSnapToPixels(settings).Should().BeTrue();
            settings.SnapToPixels.Returns(false);
            entity.ShouldSnapToPixels(settings).Should().BeTrue();
        }
    }
}