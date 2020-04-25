namespace Macabre2D.Examples.MultiPlatformTest {

    using System;

    public static class Program {

        [STAThread]
        private static void Main() {
            using (var game = new MultiPlatformGame()) {
                game.Run();
            }
        }
    }
}