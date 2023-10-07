namespace Macabresoft.Macabre2D.Tests.Framework.UI.Docking;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class DockingContainerTests {
    
    [Test]
    [Category("Unit Tests")]
    public static void DockingContainer_Initialize_ShouldCenterProperly() {
        // Arrange
        var scene = Substitute.For<IScene>();
        var boundable = new TestableBoundable() {
            BoundingArea = new BoundingArea(Vector2.Zero, new Vector2(10f))
        };

        var container = boundable.AddChild<DockingContainer>();
        container.Location = DockLocation.Center;
        var dockable = container.AddChild<DockablePanel>();
        dockable.OffsetOptions.OffsetType = PixelOffsetType.Center;
        dockable.Width = 5f;
        dockable.Height = 5f;
        
        boundable.Initialize(scene, scene);

        using (new AssertionScope()) {
            dockable.WorldPosition.Should().Be(new Vector2(5f));
        }
    }
}