using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NSubstitute;
using NUnit.Framework;

namespace Macabre2D.Tests.Framework.Tiles {

    [TestFixture]
    public static class TileableComponentTests {

        [Test]
        [Category("Unit Test")]
        public static void TileableComponent_ScaleTest() {
            using (var component = new BinaryTileMap()) {
                component.Initialize(Substitute.For<IScene>());
                var localPosition = component.GridConfiguration.Grid.GetTilePosition(new Point(1, 1));
                var worldPosition = component.WorldGrid.GetTilePosition(new Point(1, 1));

                Assert.AreEqual(localPosition, worldPosition);

                component.LocalScale = new Vector2(2f, 1f);
                localPosition = component.GridConfiguration.Grid.GetTilePosition(new Point(1, 1));
                worldPosition = component.WorldGrid.GetTilePosition(new Point(1, 1));

                Assert.AreNotEqual(localPosition, worldPosition);
                Assert.AreEqual(localPosition.X * 2f, worldPosition.X);
            }
        }
    }
}