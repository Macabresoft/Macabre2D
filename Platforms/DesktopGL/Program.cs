namespace Macabresoft.Macabre2D.Platforms.DesktopGL;

using System;
using Macabresoft.Macabre2D.Framework;

public static class Program {
    [STAThread]
    private static void Main(string[] args) {
        using var game = new BaseGame(args);
        game.Run();
    }
}