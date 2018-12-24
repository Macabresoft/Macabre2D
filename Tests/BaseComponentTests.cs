namespace Macabre2D.Tests {

    using Macabre2D.Framework;
    using NSubstitute;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public static class BaseComponentTests {

        [Test]
        [Category("Unit Test")]
        public static void BaseComponent_DrawOrderTest() {
            using (var component = new TestComponent()) {
                component.DrawOrder = 0;

                var drawOrderChanged = false;
                component.DrawOrderChanged += (object sender, EventArgs e) => drawOrderChanged = true;

                component.DrawOrder = 1;
                Assert.IsTrue(drawOrderChanged);

                drawOrderChanged = false;
                component.DrawOrder = 1;
                Assert.IsFalse(drawOrderChanged);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void BaseComponent_EnabledTest() {
            using (var parent = new TestComponent())
            using (var component = parent.AddChild<TestComponent>()) {
                parent.Initialize(Substitute.For<IScene>());
                Assert.IsTrue(component.IsEnabled);

                var enabledChanged = false;
                component.IsEnabledChanged += (object sender, EventArgs e) => enabledChanged = true;
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
        public static void BaseComponent_ResolveChildren_HasChildrenTest() {
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
        public static void BaseComponent_ResolveChildren_LateAddTest() {
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
        public static void BaseComponent_ResolveChildren_NoChildrenTest() {
            using (var component = new WithChildrenComponent()) {
                component.Initialize(Substitute.For<IScene>());
                Assert.False(component.HasTestComponent);
                Assert.IsNull(component.EmptyComponent);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void BaseComponent_UpdateOrderTest() {
            using (var component = new TestComponent()) {
                component.UpdateOrder = 0;

                var updateOrderChanged = false;
                component.UpdateOrderChanged += (object sender, EventArgs e) => updateOrderChanged = true;

                component.UpdateOrder = 1;
                Assert.IsTrue(updateOrderChanged);

                updateOrderChanged = false;
                component.UpdateOrder = 1;
                Assert.IsFalse(updateOrderChanged);
            }
        }

        [Test]
        [Category("Unit Test")]
        public static void BaseComponent_VisibleTest() {
            using (var parent = new TestComponent())
            using (var component = parent.AddChild<TestComponent>()) {
                Assert.IsTrue(component.IsVisible);

                var visibleChanged = false;
                component.IsVisibleChanged += (object sender, EventArgs e) => visibleChanged = true;
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

            [Child]
            private TestComponent _testComponent;

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