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
            PixelsPerUnit = 200
        };

        project.Fallbacks.ErrorSpritesColor = Color.Red;
        project.Fallbacks.BackgroundColor = Color.Coral;

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
        Assert.That(deserializedProject, Is.Not.Null);
        Assert.That(originalProject.ErrorSpritesColor, Is.EqualTo(deserializedProject.ErrorSpritesColor));
        Assert.That(originalProject.FallbackBackgroundColor, Is.EqualTo(deserializedProject.FallbackBackgroundColor));
        Assert.That(originalProject.PixelsPerUnit, Is.EqualTo(deserializedProject.PixelsPerUnit));
    }
}