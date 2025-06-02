namespace Macabresoft.Macabre2D.Tests.Framework.UI.Docking;

using System;
using System.Collections.Generic;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class DockingContainerTests {
    [Test]
    [Category("Unit Tests")]
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f, true)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f, true)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f, true)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f, true)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f, true)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f, true)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f, true)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f, true)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f, true)]
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f, false)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f, false)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f, false)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f, false)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f, false)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f, false)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f, false)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f, false)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f, false)]
    public static void AddChild_ShouldAlignProperly(
        DockLocation dockLocation,
        float containerSize,
        float panelSize,
        float resultX,
        float resultY,
        bool inheritParent) {
        var panels = new List<DockablePanel>();

        foreach (var offset in OffsetTypes) {
            CreateDependencies(
                dockLocation,
                offset,
                new Vector2(containerSize),
                new Vector2(panelSize),
                false,
                inheritParent,
                out var panel,
                out var container,
                out var scene);

            panels.Add(panel);
            panel.Initialize(scene, scene);
            panel.Move(new Vector2(-100f, -100f)); // Requires rearrangement.
            container.AddChild(panel);
        }

        using (new AssertionScope()) {
            foreach (var panel in panels) {
                panel.BoundingArea.Minimum.Should().BeEquivalentTo(new Vector2(resultX, resultY));
            }
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f, true)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f, true)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f, true)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f, true)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f, true)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f, true)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f, true)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f, true)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f, true)]
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f, false)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f, false)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f, false)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f, false)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f, false)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f, false)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f, false)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f, false)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f, false)]
    public static void Initialize_ShouldAlignProperly(
        DockLocation dockLocation,
        float containerSize,
        float panelSize,
        float resultX,
        float resultY,
        bool inheritParent) {
        var panels = new List<DockablePanel>();

        foreach (var offset in OffsetTypes) {
            CreateDependencies(
                dockLocation,
                offset,
                new Vector2(containerSize),
                new Vector2(panelSize),
                true,
                inheritParent,
                out var panel,
                out _,
                out _);

            panels.Add(panel);
        }

        using (new AssertionScope()) {
            foreach (var panel in panels) {
                panel.BoundingArea.Minimum.Should().BeEquivalentTo(new Vector2(resultX, resultY));
            }
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f, true)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f, true)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f, true)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f, true)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f, true)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f, true)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f, true)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f, true)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f, true)]
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f, false)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f, false)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f, false)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f, false)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f, false)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f, false)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f, false)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f, false)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f, false)]
    public static void RequestRearrange_ShouldAlignProperly(
        DockLocation dockLocation,
        float containerSize,
        float panelSize,
        float resultX,
        float resultY,
        bool inheritParent) {
        var panels = new List<DockablePanel>();

        foreach (var offset in OffsetTypes) {
            CreateDependencies(
                dockLocation,
                offset,
                new Vector2(containerSize),
                new Vector2(panelSize),
                true,
                inheritParent,
                out var panel,
                out var container,
                out _);

            panels.Add(panel);
            panel.Move(new Vector2(-100f, -100f)); // Requires rearrangement.
            container.RequestRearrange(panel);
        }

        using (new AssertionScope()) {
            foreach (var panel in panels) {
                panel.BoundingArea.Minimum.Should().BeEquivalentTo(new Vector2(resultX, resultY));
            }
        }
    }

    private static readonly IEnumerable<PixelOffsetType> OffsetTypes = Enum.GetValues<PixelOffsetType>();

    private static void CreateDependencies(
        DockLocation dockLocation,
        PixelOffsetType boundingOffsetType,
        Vector2 containerSize,
        Vector2 dockableSize,
        bool addPanelAsChild,
        bool inheritParentBoundingArea,
        out DockablePanel panel,
        out DockingContainer container,
        out IScene scene) {
        var game = Substitute.For<IGame>();
        scene = Substitute.For<IScene>();
        var project = Substitute.For<IGameProject>();
        scene.Game.Returns(game);
        project.PixelsPerUnit = 1;
        project.UnitsPerPixel.Returns(1f);
        scene.Project.Returns(project);
        game.Project.Returns(project);

        IEntity parent;

        container = new DockingContainer {
            InheritParentBoundingArea = inheritParentBoundingArea
        };

        if (inheritParentBoundingArea) {
            parent = new TestableBoundable {
                BoundingArea = new BoundingArea(Vector2.Zero, containerSize)
            };
        }
        else {
            container.Width = containerSize.X;
            container.Height = containerSize.Y;
            container.OffsetOptions.OffsetType = PixelOffsetType.BottomLeft;
            parent = new Entity();
        }

        parent.AddChild(container);


        panel = new DockablePanel {
            Width = dockableSize.X,
            Height = dockableSize.Y,
            Location = dockLocation
        };

        panel.OffsetOptions.OffsetType = boundingOffsetType;

        if (addPanelAsChild) {
            container.AddChild(panel);
        }

        parent.Initialize(scene, scene);
    }
}