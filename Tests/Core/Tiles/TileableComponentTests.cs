using Macabresoft.MonoGame.Core;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Macabresoft.MonoGame.Tests.Core.Tiles {

    [TestFixture]
    public static class TileableComponentTests {

        [Test]
        [Category("Unit Test")]
        public static void TileableComponent_ScaleTest() {
            var entity = new GameEntity();
            using (var component = entity.AddComponent<BinaryTileMap>()) {
                component.Initialize(entity);
                var localPosition = component.Grid.GetTilePosition(new Point(1, 1));
                var worldPosition = component.WorldGrid.GetTilePosition(new Point(1, 1));

                Assert.AreEqual(localPosition, worldPosition);

                entity.LocalScale = new Vector2(2f, 1f);
                localPosition = component.Grid.GetTilePosition(new Point(1, 1));
                worldPosition = component.WorldGrid.GetTilePosition(new Point(1, 1));

                Assert.AreNotEqual(localPosition, worldPosition);
                Assert.AreEqual(localPosition.X * 2f, worldPosition.X);
            }
        }
    }
}