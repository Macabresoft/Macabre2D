namespace Macabre2D.Examples.SynthTest {

    using System;

    public static class Program {

        [STAThread]
        private static void Main() {
            using (var game = new SynthGame()) {
                game.Run();
            }
        }
    }
}