namespace Macabresoft.Macabre2D.Tests.Framework;

using System.IO;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using NUnit.Framework;

[TestFixture]
public static class SerializerTests {
    [Test]
    [Category("Unit Tests")]
    public static void Serializer_GameSettingsSerializeTest() {
        var project = new GameProject {
            ErrorSpritesColor = Color.Red,
            FallbackBackgroundColor = Color.Coral,
            PixelsPerUnit = 200
        };

        var fileLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, "ProjectForTest.m2dgs");
        GameProject deserializedProject = null;
        var serializer = new Serializer();
        try {
            serializer.Serialize(project, fileLocation);
            deserializedProject = serializer.Deserialize<GameProject>(fileLocation);
        }
        finally {
            File.Delete(fileLocation);
        }

        CompareGameSettings(project, deserializedProject);

        var projectString = serializer.SerializeToString(project);
        deserializedProject = serializer.DeserializeFromString<GameProject>(projectString);

        CompareGameSettings(project, deserializedProject);
    }

    private static void CompareGameSettings(IGameProject originalProject, IGameProject deserializedProject) {
        Assert.NotNull(deserializedProject);
        Assert.AreEqual(originalProject.ErrorSpritesColor, deserializedProject.ErrorSpritesColor);
        Assert.AreEqual(originalProject.FallbackBackgroundColor, deserializedProject.FallbackBackgroundColor);
        Assert.AreEqual(originalProject.PixelsPerUnit, deserializedProject.PixelsPerUnit);
    }
}