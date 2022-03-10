namespace Macabresoft.Macabre2D.Tests.Framework;

using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public sealed class SceneTests {
    [Test]
    [Category("Unit Tests")]
    public static void ReorderSystem_FirstChildPlusOne_ShouldWork() {
        const int NumberOfChildren = 5;
        var scene = new Scene();

        for (var i = 0; i < NumberOfChildren; i++) {
            scene.AddLoop<UpdateLoop>();
        }

        var reordered = scene.Loops.ElementAt(0);
        scene.ReorderLoop(reordered, 1);

        using (new AssertionScope()) {
            scene.Loops.Count.Should().Be(NumberOfChildren);
            scene.Loops.ElementAt(1).Should().Be(reordered);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void ReorderSystem_LastChildMinusOne_ShouldWork() {
        const int NumberOfChildren = 5;
        const int OriginalIndex = NumberOfChildren - 1;
        const int NewIndex = NumberOfChildren - 2;
        var scene = new Scene();

        for (var i = 0; i < NumberOfChildren; i++) {
            scene.AddLoop<UpdateLoop>();
        }

        var reordered = scene.Loops.ElementAt(OriginalIndex);
        scene.ReorderLoop(reordered, NewIndex);

        using (new AssertionScope()) {
            scene.Loops.Count.Should().Be(NumberOfChildren);
            scene.Loops.ElementAt(NewIndex).Should().Be(reordered);
        }
    }

    [Test]
    [Category("Unit Tests")]
    public static void Scene_RegistersEntity_WhenAddedAfterInitialization() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.After);
        test.AssertExistenceOfEntities(true);
    }

    [Test]
    [Category("Unit Tests")]
    public static void Scene_RegistersEntity_WhenInitialized() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.Before);
        test.AssertExistenceOfEntities(true);
    }

    [Test]
    [Category("Unit Tests")]
    public static void Scene_UnregistersEntity_WhenRemoved() {
        var test = new SceneTestContainer(SceneTestContainer.InitializationMode.Before);

        test.Scene.RemoveChild(test.RenderableEntity);
        test.Scene.RemoveChild(test.UpdateableEntity);
        test.Scene.RemoveChild(test.CameraEntity);
        test.Scene.RemoveChild(test.UpdateableAndRenderableEntity);
        test.AssertExistenceOfEntities(false);
    }
}