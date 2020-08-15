namespace Macabre2D.Samples.Content {

    using System;

    public static class Program {

        [STAThread]
        private static void Main() {
            using var game = new ContentGame();
            game.Run();
        }
    }
}