namespace Macabresoft.Macabre2D.Tests.Framework.UI.Docking;

using System;
using System.Collections.Generic;
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
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f)]
    public static void AddChild_ShouldAlignProperly(DockLocation dockLocation, float containerSize, float panelSize, float resultX, float resultY) {
        var panels = new List<DockablePanel>();

        foreach (var offset in OffsetTypes) {
            CreateDependencies(
                dockLocation,
                offset,
                new Vector2(containerSize),
                new Vector2(panelSize),
                false,
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
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f)]
    public static void Initialize_ShouldAlignProperly(DockLocation dockLocation, float containerSize, float panelSize, float resultX, float resultY) {
        var panels = new List<DockablePanel>();

        foreach (var offset in OffsetTypes) {
            CreateDependencies(
                dockLocation,
                offset,
                new Vector2(containerSize),
                new Vector2(panelSize),
                true,
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
    [TestCase(DockLocation.Center, 10f, 5f, 2.5f, 2.5f)]
    [TestCase(DockLocation.Left, 10f, 5f, 0f, 2.5f)]
    [TestCase(DockLocation.Top, 10f, 5f, 2.5f, 5f)]
    [TestCase(DockLocation.Right, 10f, 5f, 5f, 2.5f)]
    [TestCase(DockLocation.Bottom, 10f, 5f, 2.5f, 0f)]
    [TestCase(DockLocation.BottomLeft, 10f, 5f, 0f, 0f)]
    [TestCase(DockLocation.BottomRight, 10f, 5f, 5f, 0f)]
    [TestCase(DockLocation.TopLeft, 10f, 5f, 0f, 5f)]
    [TestCase(DockLocation.TopRight, 10f, 5f, 5f, 5f)]
    public static void RequestRearrange_ShouldAlignProperly(DockLocation dockLocation, float containerSize, float panelSize, float resultX, float resultY) {
        var panels = new List<DockablePanel>();

        foreach (var offset in OffsetTypes) {
            CreateDependencies(
                dockLocation,
                offset,
                new Vector2(containerSize),
                new Vector2(panelSize),
                true,
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

        var boundable = new TestableBoundable {
            BoundingArea = new BoundingArea(Vector2.Zero, containerSize)
        };

        container = boundable.AddChild<DockingContainer>();

        panel = new DockablePanel {
            Width = dockableSize.X,
            Height = dockableSize.Y,
            Location = dockLocation
        };

        panel.OffsetOptions.OffsetType = boundingOffsetType;

        if (addPanelAsChild) {
            container.AddChild(panel);
        }

        boundable.Initialize(scene, scene);
    }
}