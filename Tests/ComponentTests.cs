namespace Macabre2D.Tests {

    using Macabre2D.Framework.Physics;
    using Macabre2D.Framework.Rendering;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public static class ComponentTests {

        [Test]
        [Category("Unit Test")]
        public static void Component_FindComponentInChildrenTest() {
            using (var component1 = new TestComponent("1"))
            using (var component2 = new TestComponent("2"))
            using (var component3 = new TestComponent("3"))
            using (var component4 = new TestComponent("4")) {
                component1.AddChild(component2);
                component2.AddChild(component3);
                component2.AddChild(component4);

                Assert.AreEqual(component2, component1.FindComponentInChildren("2"));
                Assert.AreEqual(component3, component1.FindComponentInChildren("3"));
                Assert.AreEqual(component4, component1.FindComponentInChildren("4"));
                Assert.AreEqual(component3, component2.FindComponentInChildren("3"));
                Assert.AreEqual(component4, component2.FindComponentInChildren("4"));
                Assert.Null(component2.FindComponentInChildren("1"));
                Assert.Null(component1.FindComponentInChildren("not even real"));
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_GetChildrenTest() {
            using (var component = new TestComponent()) {
                var testUpdateableComponents = new List<TestUpdateableComponent>();

                for (var i = 0; i < 5; i++) {
                    testUpdateableComponents.Add(component.AddChild<TestUpdateableComponent>());
                }

                for (var i = 0; i < 3; i++) {
                    component.AddChild<TestComponent>();
                }

                Assert.AreEqual(8, component.Children.Count);
                var gottenComponents = component.GetChildren<TestUpdateableComponent>().ToList();
                Assert.AreEqual(testUpdateableComponents.Count, gottenComponents.Count);

                foreach (var testComponent in testUpdateableComponents) {
                    Assert.Contains(testComponent, gottenComponents);
                }
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_GetChildTest() {
            using (var component = new TestComponent()) {
                var child = component.AddChild<TestComponent>();

                Assert.AreEqual(1, component.Children.Count);
                Assert.AreEqual(child, component.GetChild<TestComponent>());
                Assert.AreEqual(null, component.GetChild<TestUpdateableComponent>());
                component.AddChild<TestComponent>();
                component.AddChild<TestComponent>();
                Assert.AreEqual(3, component.Children.Count);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_GetComponentFromParent_NotInParentTest() {
            using (var parent1 = new TestComponent())
            using (var parent2 = new TestComponent())
            using (var child = new TestComponent()) {
                child.AddChild(new TestUpdateableComponent());
                child.Parent = parent2;
                parent1.Parent = parent2;

                var result = parent1.GetComponentFromParent<TestUpdateableComponent>();
                Assert.IsNull(result);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_GetComponentFromParent_OnceRemovedTest() {
            using (var component = new TestComponent())
            using (var parent1 = new TestComponent())
            using (var parent2 = new TestUpdateableComponent()) {
                parent1.Parent = parent2;
                component.Parent = parent1;

                var result = component.GetComponentFromParent<TestUpdateableComponent>();
                Assert.AreEqual(parent2, result);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_GetComponentFromParentTest() {
            using (var component = new TestComponent())
            using (var parent = new TestUpdateableComponent()) {
                component.Parent = parent;

                var result = component.GetComponentFromParent<TestUpdateableComponent>();
                Assert.AreEqual(parent, result);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_GetComponentInChildrenTest() {
            using (var component1 = new TestComponent("1"))
            using (var component2 = new TestComponent("2"))
            using (var component3 = new TestComponent("3"))
            using (var component4 = new TestComponent("4")) {
                component1.AddChild(component2);
                component2.AddChild(component3);
                component3.AddChild(component4);

                var testComponent = new TestUpdateableComponent();
                component4.AddChild(testComponent);
                component2.AddChild(new SpriteAnimator());
                component3.AddChild(new Body());
                component4.AddChild(new SpriteAnimator());

                Assert.AreEqual(testComponent, component1.GetComponentInChildren<TestUpdateableComponent>(false));
                Assert.IsNull(component1.GetComponentInChildren<TestUpdateableComponent>(true));
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_GetComponentsInChildrenTest() {
            using (var component1 = new TestComponent())
            using (var component2 = new TestComponent())
            using (var component3 = new TestComponent())
            using (var component4 = new TestComponent()) {
                component2.Parent = component1;
                component3.Parent = component2;
                component4.Parent = component2;

                var testComponents = new TestUpdateableComponent[10];
                var sequencers = new SpriteAnimator[10];

                for (var i = 0; i < 10; i++) {
                    if (i % 4 == 0) {
                        testComponents[i] = component1.AddChild<TestUpdateableComponent>();
                        sequencers[i] = component1.AddChild<SpriteAnimator>();
                    }
                    else if (i % 3 == 0) {
                        testComponents[i] = component2.AddChild<TestUpdateableComponent>();
                        sequencers[i] = component2.AddChild<SpriteAnimator>();
                    }
                    else if (i % 2 == 0) {
                        testComponents[i] = component3.AddChild<TestUpdateableComponent>();
                        sequencers[i] = component3.AddChild<SpriteAnimator>();
                    }
                    else {
                        testComponents[i] = component4.AddChild<TestUpdateableComponent>();
                        sequencers[i] = component4.AddChild<SpriteAnimator>();
                    }
                }

                var result1 = component1.GetComponentsInChildren<TestUpdateableComponent>();
                Assert.AreEqual(result1.Count, testComponents.Length);
                foreach (var component in testComponents) {
                    Assert.Contains(component, result1);
                }

                var result2 = component1.GetComponentsInChildren<SpriteAnimator>();
                Assert.AreEqual(result2.Count, sequencers.Length);
                foreach (var component in sequencers) {
                    Assert.Contains(component, result2);
                }
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_ParentChangedTest() {
            using (var parent = new TestComponent())
            using (var child = new TestComponent()) {
                var parentChangedCalled1 = false;

                Assert.NotNull(child);

                child.ParentChanged += (sender, e) => parentChangedCalled1 = e == parent;

                child.Parent = parent;

                Assert.True(parentChangedCalled1);
                Assert.AreEqual(child.Parent, parent);
                Assert.AreEqual(1, parent.Children.Count);

                var parentChangedCalled2 = false;

                child.ParentChanged += (sender, e) => parentChangedCalled2 = e == null;

                child.Parent = null;

                Assert.True(parentChangedCalled2);
                Assert.IsNull(child.Parent);
                Assert.IsEmpty(parent.Children);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_SetWorldPositionTest() {
            using (var component1 = new TestComponent("1"))
            using (var component2 = new TestComponent("2")) {
                component1.LocalPosition = new Vector2(10f, 3f);
                component2.Parent = component1;

                component2.SetWorldPosition(new Vector2(-10f, 0f));
                Assert.AreEqual(new Vector2(-10f, 0f), component2.WorldTransform.Position);

                component1.SetWorldPosition(Vector2.Zero);
                Assert.AreEqual(Vector2.Zero, component1.WorldTransform.Position);
            }
        }
    }
}