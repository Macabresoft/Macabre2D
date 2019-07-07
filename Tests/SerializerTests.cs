namespace Macabre2D.Tests {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;
    using System;
    using System.IO;

    [TestFixture]
    public static class SerializerTests {

        [Test]
        [Category("Unit Test")]
        public static void Serializer_GameSettingsSerializeTest() {
            var gameSettings = new GameSettings() {
                ErrorSpritesColor = Color.Red,
                FallbackBackgroundColor = Color.Coral,
                PixelsPerUnit = 200,
                StartupSceneContentId = Guid.NewGuid()
            };

            var fileLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, "GameSettingsForTest.m2dgs");
            GameSettings deserializedGameSettings = null;
            var serializer = new Serializer();
            try {
                serializer.Serialize(gameSettings, fileLocation);
                deserializedGameSettings = serializer.Deserialize<GameSettings>(fileLocation);
            }
            finally {
                File.Delete(fileLocation);
            }

            CompareGameSettings(gameSettings, deserializedGameSettings);

            var gameSettingsString = serializer.SerializeToString(gameSettings);
            deserializedGameSettings = serializer.DeserializeFromString<GameSettings>(gameSettingsString);

            CompareGameSettings(gameSettings, deserializedGameSettings);
        }

        private static void CompareGameSettings(IGameSettings originalSettings, IGameSettings deserializedSettings) {
            Assert.NotNull(deserializedSettings);
            Assert.AreEqual(originalSettings.ErrorSpritesColor, deserializedSettings.ErrorSpritesColor);
            Assert.AreEqual(originalSettings.FallbackBackgroundColor, deserializedSettings.FallbackBackgroundColor);
            Assert.AreEqual(originalSettings.PixelsPerUnit, deserializedSettings.PixelsPerUnit);
            Assert.AreEqual(originalSettings.StartupSceneContentId, deserializedSettings.StartupSceneContentId);
        }
    }
}