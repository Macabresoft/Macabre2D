namespace Macabre2D.Tests.Framework {

    using Macabre2D.Framework;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public static class SceneTests {

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_AddsModuleDuringInitializeTest() {
            using (var scene = new Scene())
            using (var component = new ModuleOwningTestComponent()) {
                scene.AddComponent(component);
                scene.Initialize();
                Assert.True(component.IsInitialized);
                Assert.True(component.Module.IsInitialized);
            }
        }

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

                scene.ComponentCreated += (object sender, BaseComponent e) => parentHasBeenAdded = parentHasBeenAdded || e == parent;
                scene.ComponentCreated += (object sender, BaseComponent e) => child1HasBeenAdded = child1HasBeenAdded || e == child1;
                scene.ComponentCreated += (object sender, BaseComponent e) => child2HasBeenAdded = child2HasBeenAdded || e == child2;

                scene.Initialize();
                scene.AddComponent(parent);

                Assert.True(parentHasBeenAdded);
                Assert.True(child1HasBeenAdded);
                Assert.True(child2HasBeenAdded);
                Assert.False(scene.Components.Contains(child1));
                Assert.False(scene.Components.Contains(child2));
                Assert.Contains(parent, scene.Components.ToList());
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

                scene.ComponentCreated += (object sender, BaseComponent e) => parentHasBeenAdded = parentHasBeenAdded || e == parent;
                scene.ComponentCreated += (object sender, BaseComponent e) => child1HasBeenAdded = child1HasBeenAdded || e == child1;
                scene.ComponentCreated += (object sender, BaseComponent e) => child2HasBeenAdded = child2HasBeenAdded || e == child2;

                scene.AddComponent(parent);

                Assert.False(parentHasBeenAdded);
                Assert.False(child1HasBeenAdded);
                Assert.False(child2HasBeenAdded);

                scene.Initialize();

                Assert.True(parentHasBeenAdded);
                Assert.True(child1HasBeenAdded);
                Assert.True(child2HasBeenAdded);
                Assert.False(scene.Components.Contains(child1));
                Assert.False(scene.Components.Contains(child2));
                Assert.Contains(parent, scene.Components.ToList());
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

                scene.ComponentCreated += (object sender, BaseComponent e) => hasBeenAdded = hasBeenAdded || e == child;
                scene.Initialize();

                Assert.True(hasBeenAdded);
                Assert.False(scene.Components.Contains(child));
                Assert.Contains(parent, scene.Components.ToList());
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

                scene.ComponentCreated += (object sender, BaseComponent e) => hasBeenAdded = hasBeenAdded || e == child;
                Assert.False(hasBeenAdded);

                scene.Initialize();

                Assert.True(hasBeenAdded);
                Assert.False(scene.Components.Contains(child));
                Assert.Contains(parent, scene.Components.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_NullParent_PostInitializeTest() {
            using (var scene = new Scene())
            using (var component = new TestComponent()) {
                var hasBeenAdded = false;

                scene.ComponentCreated += (object sender, BaseComponent e) => hasBeenAdded = hasBeenAdded || e == component;

                scene.Initialize();
                scene.AddComponent(component);

                Assert.True(hasBeenAdded);
                Assert.Contains(component, scene.Components.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_AddComponent_NullParent_PreInitializeTest() {
            using (var scene = new Scene())
            using (var component = new TestComponent()) {
                var hasBeenAdded = false;

                scene.ComponentCreated += (object sender, BaseComponent e) => hasBeenAdded = hasBeenAdded || e == component;
                scene.AddComponent(component);

                Assert.False(hasBeenAdded);

                scene.Initialize();

                Assert.True(hasBeenAdded);
                Assert.Contains(component, scene.Components.ToList());
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Scene_DestroyComponent_NullParentTest() {
            using (var scene = new Scene())
            using (var component = new TestComponent()) {
                scene.AddComponent(component);
                scene.Initialize();
                Assert.Contains(component, scene.Components.ToList());

                var hasBeenRemoved = false;
                scene.ComponentDestroyed += (object sender, BaseComponent e) => hasBeenRemoved = hasBeenRemoved || e == component;
                scene.DestroyComponent(component);

                Assert.True(hasBeenRemoved);
                Assert.False(scene.Components.Contains(component));
            }
        }
    }
}