namespace Macabresoft.Macabre2D.Samples.Physics {

    using System;

    public static class Program {

        [STAThread]
        private static void Main() {
            using var game = new PhysicsGame();
            game.Run();
        }
    }
}