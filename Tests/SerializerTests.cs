namespace Macabre2D.Tests {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Serialization;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;
    using System.IO;

    [TestFixture]
    public static class SerializerTests {

        [Test]
        [Category("Unit Test")]
        public static void Serializer_GameSettingsSerializeTest() {
            var gameSettings = new GameSettings() {
                FallbackBackgroundColor = Color.Coral,
                PixelsPerUnit = 200,
                StartupScenePath = "ThisSceneIsAHoldUp"
            };

            var fileLocation = "GameSettingsForTest.m2dgs";
            GameSettings deserializedGameSettings = null;
            var serializer = new Serializer();
            try {
                serializer.Serialize(gameSettings, fileLocation);
                deserializedGameSettings = serializer.Deserialize<GameSettings>(fileLocation);
            }
            finally {
                File.Delete(fileLocation);
            }

            Assert.NotNull(deserializedGameSettings);
            Assert.AreEqual(gameSettings.FallbackBackgroundColor, deserializedGameSettings.FallbackBackgroundColor);
            Assert.AreEqual(gameSettings.PixelsPerUnit, deserializedGameSettings.PixelsPerUnit);
            Assert.AreEqual(gameSettings.StartupScenePath, deserializedGameSettings.StartupScenePath);

            deserializedGameSettings = null;

            var gameSettingsString = serializer.SerializeToString(gameSettings);
            deserializedGameSettings = serializer.DeserializeFromString<GameSettings>(gameSettingsString);

            Assert.NotNull(deserializedGameSettings);
            Assert.AreEqual(gameSettings.FallbackBackgroundColor, deserializedGameSettings.FallbackBackgroundColor);
            Assert.AreEqual(gameSettings.PixelsPerUnit, deserializedGameSettings.PixelsPerUnit);
            Assert.AreEqual(gameSettings.StartupScenePath, deserializedGameSettings.StartupScenePath);
        }
    }
}