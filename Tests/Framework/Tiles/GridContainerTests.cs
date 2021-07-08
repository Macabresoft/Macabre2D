namespace Macabresoft.Macabre2D.Tests.Framework.Tiles {
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public sealed class GridContainerTests {
        private sealed class GridContainerUser : Entity {
            public IGridContainer Container { get; private set; } = GridContainer.EmptyGridContainer;

            public override void Initialize(IScene scene, IEntity parent) {
                base.Initialize(scene, parent);

                if (this.TryGetParentEntity(out IGridContainer container)) {
                    this.Container = container;
                }
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(100)]
        public static void Entity_ShouldGetGridContainer_AtDepth(int depth) {
            var scene = Substitute.For<IScene>();
            var container = new GridContainer();
            var parent = container.AddChild<Entity>();

            for (var i = 1; i <= depth; i++) {
                parent = parent.AddChild<Entity>();
            }

            var child = parent.AddChild<GridContainerUser>();
            container.Initialize(scene, scene);

            using (new AssertionScope()) {
                child.Container.Should().Be(container);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public static void Entity_ShouldGetGridContainer_WhenDirectParent() {
            var scene = Substitute.For<IScene>();
            var container = new GridContainer();
            var child = container.AddChild<GridContainerUser>();
            container.Initialize(scene, scene);

            using (new AssertionScope()) {
                child.Container.Should().Be(container);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public static void Entity_ShouldGetScene_WhenDirectParent() {
            var scene = Substitute.For<IScene>();
            var child = new GridContainerUser();
            child.Initialize(scene, scene);

            using (new AssertionScope()) {
                child.Container.Should().Be(scene);
            }
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(100)]
        public static void Entity_ShouldGetSceneGridContainer_AtDepth(int depth) {
            var scene = Substitute.For<IScene>();
            var rootParent = new Entity();
            var parent = rootParent.AddChild<Entity>();

            for (var i = 1; i <= depth; i++) {
                parent = parent.AddChild<Entity>();
            }

            var child = parent.AddChild<GridContainerUser>();
            rootParent.Initialize(scene, scene);

            using (new AssertionScope()) {
                child.Container.Should().Be(scene);
            }
        }
    }
}