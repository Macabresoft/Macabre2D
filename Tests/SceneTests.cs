namespace Macabre2D.Tests {

    using Macabre2D.Framework;
    using NSubstitute;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public static class SceneTests {

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_HasChildren_PostInitializeTest() {
            using (var scene = new Scene())
            using (var child1 = new TestComponent())
            using (var child2 = new TestComponent())
            using (var parent = new TestComponent()) {
                parent.AddChild(child1);
                parent.AddChild(child2);

                var parentHasBeenAdded = false;
                var child1HasBeenAdded = false;
                var child2HasBeenAdded = false;

                scene.ComponentAdded += (object sender, BaseComponent e) => parentHasBeenAdded = parentHasBeenAdded || e == parent;
                scene.ComponentAdded += (object sender, BaseComponent e) => child1HasBeenAdded = child1HasBeenAdded || e == child1;
                scene.ComponentAdded += (object sender, BaseComponent e) => child2HasBeenAdded = child2HasBeenAdded || e == child2;

                scene.Initialize(Substitute.For<IGame>());
                scene.AddComponent(parent);

                Assert.True(parentHasBeenAdded);
                Assert.True(child1HasBeenAdded);
                Assert.True(child2HasBeenAdded);
                Assert.False(scene.Children.Contains(child1));
                Assert.False(scene.Children.Contains(child2));
                Assert.Contains(parent, scene.Children.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_HasChildren_PreInitializeTest() {
            using (var scene = new Scene())
            using (var child1 = new TestComponent())
            using (var child2 = new TestComponent())
            using (var parent = new TestComponent()) {
                parent.AddChild(child1);
                parent.AddChild(child2);

                var parentHasBeenAdded = false;
                var child1HasBeenAdded = false;
                var child2HasBeenAdded = false;

                scene.ComponentAdded += (object sender, BaseComponent e) => parentHasBeenAdded = parentHasBeenAdded || e == parent;
                scene.ComponentAdded += (object sender, BaseComponent e) => child1HasBeenAdded = child1HasBeenAdded || e == child1;
                scene.ComponentAdded += (object sender, BaseComponent e) => child2HasBeenAdded = child2HasBeenAdded || e == child2;

                scene.AddComponent(parent);

                Assert.False(parentHasBeenAdded);
                Assert.False(child1HasBeenAdded);
                Assert.False(child2HasBeenAdded);

                scene.Initialize(Substitute.For<IGame>());

                Assert.True(parentHasBeenAdded);
                Assert.True(child1HasBeenAdded);
                Assert.True(child2HasBeenAdded);
                Assert.False(scene.Children.Contains(child1));
                Assert.False(scene.Children.Contains(child2));
                Assert.Contains(parent, scene.Children.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_HasParent_PostInitializeTest() {
            using (var scene = new Scene())
            using (var child = new TestComponent())
            using (var parent = new TestComponent()) {
                scene.AddComponent(parent);
                child.Parent = parent;

                var hasBeenAdded = false;

                scene.ComponentAdded += (object sender, BaseComponent e) => hasBeenAdded = hasBeenAdded || e == child;
                scene.Initialize(Substitute.For<IGame>());

                Assert.True(hasBeenAdded);
                Assert.False(scene.Children.Contains(child));
                Assert.Contains(parent, scene.Children.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_HasParent_PreInitializeTest() {
            using (var scene = new Scene())
            using (var child = new TestComponent())
            using (var parent = new TestComponent()) {
                scene.AddComponent(parent);
                child.Parent = parent;

                var hasBeenAdded = false;

                scene.ComponentAdded += (object sender, BaseComponent e) => hasBeenAdded = hasBeenAdded || e == child;
                Assert.False(hasBeenAdded);

                scene.Initialize(Substitute.For<IGame>());

                Assert.True(hasBeenAdded);
                Assert.False(scene.Children.Contains(child));
                Assert.Contains(parent, scene.Children.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_NullParent_PostInitializeTest() {
            using (var scene = new Scene())
            using (var component = new TestComponent()) {
                var hasBeenAdded = false;

                scene.ComponentAdded += (object sender, BaseComponent e) => hasBeenAdded = hasBeenAdded || e == component;
                scene.Initialize(Substitute.For<IGame>());
                scene.AddComponent(component);

                Assert.True(hasBeenAdded);
                Assert.Contains(component, scene.Children.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_NullParent_PreInitializeTest() {
            using (var scene = new Scene())
            using (var component = new TestComponent()) {
                var hasBeenAdded = false;

                scene.ComponentAdded += (object sender, BaseComponent e) => hasBeenAdded = hasBeenAdded || e == component;
                scene.AddComponent(component);

                Assert.False(hasBeenAdded);

                scene.Initialize(Substitute.For<IGame>());

                Assert.True(hasBeenAdded);
                Assert.Contains(component, scene.Children.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_DestroyComponent_NullParentTest() {
            using (var scene = new Scene())
            using (var component = new TestComponent()) {
                scene.AddComponent(component);
                scene.Initialize(Substitute.For<IGame>());
                Assert.Contains(component, scene.Children.ToList());

                var hasBeenRemoved = false;
                scene.ComponentRemoved += (object sender, BaseComponent e) => hasBeenRemoved = hasBeenRemoved || e == component;
                scene.DestroyComponent(component);

                Assert.True(hasBeenRemoved);
                Assert.False(scene.Children.Contains(component));
            }
        }
    }
}