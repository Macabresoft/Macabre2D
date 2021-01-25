namespace Macabresoft.Macabre2D.Tests.Framework.Tiles {
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;

    [TestFixture]
    public static class GridComponentTests {
        [Test]
        [Category("Unit Tests")]
        public static void GridComponent_ScaleTest() {
            var entity = new GameEntity();
            using var gridComponent = entity.AddComponent<GridComponent>();
            gridComponent.Initialize(entity);
            var localPosition = gridComponent.Grid.GetTilePosition(new Point(1, 1));
            var worldPosition = gridComponent.WorldGrid.GetTilePosition(new Point(1, 1));

            Assert.AreEqual(localPosition, worldPosition);

            entity.LocalScale = new Vector2(2f, 1f);
            localPosition = gridComponent.Grid.GetTilePosition(new Point(1, 1));
            worldPosition = gridComponent.WorldGrid.GetTilePosition(new Point(1, 1));

            Assert.AreNotEqual(localPosition, worldPosition);
            Assert.AreEqual(localPosition.X * 2f, worldPosition.X);
        }
    }
}