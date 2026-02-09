namespace Macabre2D.Tests.Framework;

using Macabre2D.Framework;
using Macabre2D.Project.Common;
using NSubstitute;

public static class GameHelpers {
    public static IGame CreateGameSubstitute() {
        var game = Substitute.For<IGame>();
        game.Project.Returns(new GameProject());
        game.State.Returns(new GameState());
        game.UserSettings.Returns(new UserSettings());
        return game;
    }
}