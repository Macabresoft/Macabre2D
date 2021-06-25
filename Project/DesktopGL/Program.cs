namespace Macabre2D.Project.DesktopGL {
    using System;
    using Macabresoft.Macabre2D.Framework;

    public static class Program {
        [STAThread]
        private static void Main() {
            using var game = new BaseGame();
            game.Run();
        }
    }
}