﻿namespace Macabresoft.Macabre2D.Tests.Framework.UI.Docking;

using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public static class DockableWrapperTests {
    [Test]
    [Category("Unit Tests")]
    public static void Initialize_Should_CombineBoundingAreas() {
        var boundingAreaA = new TestableBoundable {
            BoundingArea = new BoundingArea(Vector2.Zero, new Vector2(10f))
        };

        var boundingAreaB = new TestableBoundable {
            BoundingArea = new BoundingArea(new Vector2(-5f), new Vector2(1f))
        };

        var wrapper = CreateWrapper(boundingAreaA, boundingAreaB);

        using (new AssertionScope()) {
            wrapper.BoundingArea.Maximum.Should().Be(boundingAreaA.BoundingArea.Maximum);
            wrapper.BoundingArea.Minimum.Should().Be(boundingAreaB.BoundingArea.Minimum);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void Move_Should_OnlyResetBoundingAreaOnce() {
        var children = new IEntity[10];
        var childChangeCalls = 0;
        for (var i = 0; i < children.Length; i++) {
            var child = new DockablePanel { Width = i + 1f, Height = i + 1f };
            child.BoundingAreaChanged += (_, _) => childChangeCalls++;
            children[i] = child;
        }

        var wrapper = CreateWrapper(children);
        var wrapperChangeCalls = 0;
        childChangeCalls = 0; // Reset it, because initialize will increase it.
        wrapper.BoundingAreaChanged += (_, _) => wrapperChangeCalls++;
        wrapper.Move(Vector2.One);

        using (new AssertionScope()) {
            childChangeCalls.Should().Be(children.Length);
            wrapperChangeCalls.Should().Be(1);
        }
    }

    private static DockableWrapper CreateWrapper(params IEntity[] children) {
        var scene = Substitute.For<IScene>();
        var wrapper = new DockableWrapper();

        foreach (var child in children) {
            wrapper.AddChild(child);
        }

        wrapper.Initialize(scene, scene);
        return wrapper;
    }
}