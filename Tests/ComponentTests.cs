namespace Macabre2D.Framework.Tests {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using CategoryAttribute = NUnit.Framework.CategoryAttribute;

    [TestFixture]
    public static class ComponentTests {

        [Test]
        [Category("Unit Test")]
        public static void Component_Clone_HasChildTest() {
            using (var component = new TestComponent())
            using (var child = new TestComponent()) {
                component.Initialize(Substitute.For<IScene>());

                var random = new Random();
                child.LocalPosition = new Vector2(random.Next(-1000, 1000) / 3f, random.Next(-1000, 1000) / 3f);
                child.LocalScale = new Vector2(random.Next(-1000, 1000) / 3f, random.Next(-1000, 1000) / 3f);

                component.AddChild(child);

                using (var clone = component.Clone()) {
                    Assert.AreNotEqual(component.Id, clone.Id);

                    var cloneChild = clone.Children.First();
                    Assert.AreEqual(child.LocalPosition, cloneChild.LocalPosition);
                    Assert.AreEqual(child.LocalScale, cloneChild.LocalScale);
                    Assert.AreNotEqual(child.Id, cloneChild.Id);
                }
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_Clone_HasParentTest() {
            using (var parent = new TestComponent())
            using (var component = new TestComponent()) {
                parent.Initialize(Substitute.For<IScene>());

                var random = new Random();
                component.LocalPosition = new Vector2(random.Next(-1000, 1000) / 3f, random.Next(-1000, 1000) / 3f);
                component.LocalScale = new Vector2(random.Next(-1000, 1000) / 3f, random.Next(-1000, 1000) / 3f);

                parent.AddChild(component);

                using (var clone = component.Clone()) {
                    Assert.AreNotEqual(component.Id, clone.Id);
                    Assert.NotNull(clone.Parent);
                    Assert.AreEqual(component.Parent.Id, clone.Parent.Id);
                }
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_Clone_NoParentOrChildrenTest() {
            using (var component = new TestComponent()) {
                var random = new Random();
                component.LocalPosition = new Vector2(random.Next(-1000, 1000) / 3f, random.Next(-1000, 1000) / 3f);
                component.LocalScale = new Vector2(random.Next(-1000, 1000) / 3f, random.Next(-1000, 1000) / 3f);

                using (var clone = component.Clone()) {
                    Assert.AreNotEqual(component.Id, clone.Id);
                    Assert.AreEqual(component.LocalPosition, clone.LocalPosition);
                    Assert.AreEqual(component.LocalScale, clone.LocalScale);
                }
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_DrawOrderTest() {
            using (var component = new TestComponent()) {
                component.DrawOrder = 0;

                var drawOrderChanged = false;
                component.PropertyChanged += (object sender, PropertyChangedEventArgs e) => drawOrderChanged |= e.PropertyName == nameof(component.DrawOrder);

                component.DrawOrder = 1;
                Assert.IsTrue(drawOrderChanged);

                drawOrderChanged = false;
                component.DrawOrder = 1;
                Assert.IsFalse(drawOrderChanged);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_EnabledTest() {
            using (var parent = new TestComponent())
            using (var component = parent.AddChild<TestComponent>()) {
                parent.Initialize(Substitute.For<IScene>());
                Assert.IsTrue(component.IsEnabled);

                var enabledChanged = false;
                component.PropertyChanged += (object sender, PropertyChangedEventArgs e) => enabledChanged |= e.PropertyName == nameof(component.IsEnabled);
                parent.IsEnabled = false;
                Assert.IsTrue(enabledChanged);
                Assert.IsFalse(component.IsEnabled);

                enabledChanged = false;
                parent.IsEnabled = true;
                Assert.IsTrue(enabledChanged);
                Assert.IsTrue(component.IsEnabled);

                enabledChanged = false;
                component.IsEnabled = false;
                Assert.IsTrue(enabledChanged);
                Assert.IsFalse(component.IsEnabled);

                enabledChanged = false;
                parent.IsEnabled = false;
                Assert.IsFalse(enabledChanged);
                Assert.IsFalse(component.IsEnabled);
            }
        }

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
                component2.AddChild(new SpriteAnimationComponent());
                component3.AddChild(new SimpleBodyComponent());
                component4.AddChild(new SpriteAnimationComponent());

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
                var sequencers = new SpriteAnimationComponent[10];

                for (var i = 0; i < 10; i++) {
                    if (i % 4 == 0) {
                        testComponents[i] = component1.AddChild<TestUpdateableComponent>();
                        sequencers[i] = component1.AddChild<SpriteAnimationComponent>();
                    }
                    else if (i % 3 == 0) {
                        testComponents[i] = component2.AddChild<TestUpdateableComponent>();
                        sequencers[i] = component2.AddChild<SpriteAnimationComponent>();
                    }
                    else if (i % 2 == 0) {
                        testComponents[i] = component3.AddChild<TestUpdateableComponent>();
                        sequencers[i] = component3.AddChild<SpriteAnimationComponent>();
                    }
                    else {
                        testComponents[i] = component4.AddChild<TestUpdateableComponent>();
                        sequencers[i] = component4.AddChild<SpriteAnimationComponent>();
                    }
                }

                var result1 = component1.GetComponentsInChildren<TestUpdateableComponent>();
                Assert.AreEqual(result1.Count, testComponents.Length);
                foreach (var component in testComponents) {
                    Assert.Contains(component, result1);
                }

                var result2 = component1.GetComponentsInChildren<SpriteAnimationComponent>();
                Assert.AreEqual(result2.Count, sequencers.Length);
                foreach (var component in sequencers) {
                    Assert.Contains(component, result2);
                }
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_IsAncestorOfTest() {
            using (var component = new TestComponent())
            using (var child1 = new TestComponent())
            using (var child1A = new TestComponent())
            using (var child1B = new TestComponent())
            using (var child1A1 = new TestComponent())
            using (var child2 = new TestComponent())
            using (var child3 = new TestComponent())
            using (var child3A = new TestComponent())
            using (var looseComponent = new TestComponent()) {
                var scene = Substitute.For<IScene>();
                component.Initialize(scene);
                looseComponent.Initialize(scene);

                component.AddChild(child1);
                child1A.Parent = child1;
                child1B.Parent = child1;
                child1A1.Parent = child1A;
                child2.Parent = component;
                child3.Parent = component;
                child3A.Parent = child3;

                Assert.AreEqual(3, component.Children.Count);

                Assert.True(component.IsAncestorOf(child1));
                Assert.True(component.IsAncestorOf(child1A));
                Assert.True(component.IsAncestorOf(child1B));
                Assert.True(component.IsAncestorOf(child1A1));
                Assert.True(component.IsAncestorOf(child2));
                Assert.True(component.IsAncestorOf(child3));
                Assert.True(component.IsAncestorOf(child3A));
                Assert.False(component.IsAncestorOf(looseComponent));

                Assert.False(looseComponent.IsAncestorOf(child1));
                Assert.False(looseComponent.IsAncestorOf(child1A));
                Assert.False(looseComponent.IsAncestorOf(child1B));
                Assert.False(looseComponent.IsAncestorOf(child1A1));
                Assert.False(looseComponent.IsAncestorOf(child2));
                Assert.False(looseComponent.IsAncestorOf(child3));
                Assert.False(looseComponent.IsAncestorOf(child3A));
                Assert.False(looseComponent.IsAncestorOf(component));

                Assert.True(child1A.IsAncestorOf(child1A1));
                Assert.True(child3.IsAncestorOf(child3A));

                Assert.False(component.IsAncestorOf(component));
                Assert.False(child1A1.IsAncestorOf(child1A));

                Assert.True(child1.IsAncestorOf(child1A));
                Assert.True(child1.IsAncestorOf(child1A1));
                Assert.True(child1.IsAncestorOf(child1B));

                Assert.False(component.IsAncestorOf(null));
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_IsDescendentOfTest() {
            using (var component = new TestComponent())
            using (var child1 = new TestComponent())
            using (var child1A = new TestComponent())
            using (var child1A1 = new TestComponent())
            using (var child2 = new TestComponent())
            using (var looseComponent = new TestComponent()) {
                var scene = Substitute.For<IScene>();
                component.Initialize(scene);
                looseComponent.Initialize(scene);

                child1.Parent = component;
                child1A.Parent = child1;
                child1A1.Parent = child1A;
                child2.Parent = component;

                Assert.True(child1.IsDescendentOf(component));
                Assert.True(child1A.IsDescendentOf(component));
                Assert.True(child1A1.IsDescendentOf(component));
                Assert.True(child2.IsDescendentOf(component));

                Assert.False(looseComponent.IsDescendentOf(component));
                Assert.False(child2.IsDescendentOf(child1));
                Assert.False(child1A.IsDescendentOf(child1A1));
                Assert.False(child2.IsDescendentOf(null));
                Assert.False(component.IsDescendentOf(component));
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
        public static void Component_ResolveChildren_HasChildrenTest() {
            using (var component = new WithChildrenComponent()) {
                component.AddChild<EmptyComponent>();
                component.AddChild<TestComponent>();

                component.Initialize(Substitute.For<IScene>());
                Assert.True(component.HasTestComponent);
                Assert.IsNotNull(component.EmptyComponent);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_ResolveChildren_LateAddTest() {
            using (var component = new WithChildrenComponent()) {
                component.Initialize(Substitute.For<IScene>());
                Assert.False(component.HasTestComponent);
                Assert.IsNull(component.EmptyComponent);

                component.AddChild<TestComponent>();
                Assert.True(component.HasTestComponent);

                component.AddChild<EmptyComponent>();
                Assert.IsNotNull(component.EmptyComponent);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_ResolveChildren_NoChildrenTest() {
            using (var component = new WithChildrenComponent()) {
                component.Initialize(Substitute.For<IScene>());
                Assert.False(component.HasTestComponent);
                Assert.IsNull(component.EmptyComponent);
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

        [Test]
        [Category("Unit Test")]
        public static void Component_Trasnform_NoParentTest() {
            using (var component = new TestComponent()) {
                component.Initialize(Substitute.For<IScene>());

                var random = new Random();
                var floatDelta = 0.0001f;

                for (var i = 0; i < 100; i++) {
                    component.LocalPosition = new Vector2(random.Next(-1000, 1000) / 3f, random.Next(-1000, 1000) / 3f);
                    Assert.AreEqual(component.LocalPosition.X, component.WorldTransform.Position.X, floatDelta);
                    Assert.AreEqual(component.LocalPosition.Y, component.WorldTransform.Position.Y, floatDelta);
                }

                component.LocalPosition = Vector2.One;

                for (var i = 0; i < 100; i++) {
                    component.LocalScale = new Vector2(random.Next(-1000, -1) / 3f, random.Next(-1000, -1) / 3f);
                    Assert.AreEqual(component.LocalScale.X, component.WorldTransform.Scale.X, floatDelta);
                    Assert.AreEqual(component.LocalScale.Y, component.WorldTransform.Scale.Y, floatDelta);
                }
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_UpdateOrderTest() {
            using (var component = new TestComponent()) {
                component.UpdateOrder = 0;

                var updateOrderChanged = false;
                component.PropertyChanged += (object sender, PropertyChangedEventArgs e) => updateOrderChanged |= e.PropertyName == nameof(component.UpdateOrder);

                component.UpdateOrder = 1;
                Assert.IsTrue(updateOrderChanged);

                updateOrderChanged = false;
                component.UpdateOrder = 1;
                Assert.IsFalse(updateOrderChanged);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void Component_VisibleTest() {
            using (var parent = new TestComponent())
            using (var component = parent.AddChild<TestComponent>()) {
                Assert.IsTrue(component.IsVisible);

                var visibleChanged = false;
                component.PropertyChanged += (object sender, PropertyChangedEventArgs e) => visibleChanged |= e.PropertyName == nameof(component.IsVisible);
                component.IsVisible = false;
                Assert.IsTrue(visibleChanged);
                Assert.IsFalse(component.IsVisible);

                visibleChanged = false;
                component.IsEnabled = false;
                Assert.IsFalse(visibleChanged);
                Assert.IsFalse(component.IsVisible);

                visibleChanged = false;
                component.IsEnabled = true;
                Assert.IsFalse(visibleChanged);
                Assert.IsFalse(component.IsVisible);

                visibleChanged = false;
                component.IsVisible = true;
                Assert.IsTrue(visibleChanged);
                Assert.IsTrue(component.IsVisible);

                visibleChanged = false;
                component.IsEnabled = false;
                Assert.IsTrue(visibleChanged);
                Assert.IsFalse(component.IsVisible);
            }
        }

        private class WithChildrenComponent : BaseComponent {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Never assigned to

            [Child]
            private TestComponent _testComponent;

#pragma warning restore CS0649 // Never assigned to
#pragma warning restore IDE0044 // Add readonly modifier

            [Child]
            public EmptyComponent EmptyComponent { get; set; }

            public bool HasTestComponent { get { return this._testComponent != null; } }

            protected override void Initialize() {
                return;
            }
        }
    }
}