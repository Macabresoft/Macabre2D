namespace Macabre2D.Project.DesktopGL {

    using Macabre2D.Project.Gameplay;
    using System;

    public static class Program {

        [STAThread]
        private static void Main() {
            using var game = new CustomGame();
            game.Run();
        }
    }
}