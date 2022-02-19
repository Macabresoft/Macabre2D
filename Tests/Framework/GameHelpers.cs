namespace Macabresoft.Macabre2D.Tests.Framework;

using Macabresoft.Macabre2D.Framework;
using NSubstitute;

public static class GameHelpers {
    public static IGame CreateGameSubstitute() {
        var game = Substitute.For<IGame>();
        game.Project.Returns(new GameProject());
        return game;
    }
}