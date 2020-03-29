namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using System;

    public static class SceneHelper {
        public const byte SceneHeight = 36;

        public static IScene CreateSynthScene() {
            // Add pieno keys, current position bar, play, pause, and stop buttons
            throw new NotImplementedException();
        }

        public static IScene CreateSynthScene(Song song) {
            var scene = SceneHelper.CreateSynthScene();
            // add individual notess to scene
            throw new NotImplementedException();
        }

        public static float GetRowPosition(Note note, Pitch pitch) {
            var pitchMultiplier = SceneHelper.GetPitchMultiplier(pitch);
            return 12f * pitchMultiplier + (byte)note;
        }

        private static float GetPitchMultiplier(Pitch pitch) {
            var result = 0f;
            if (pitch == Pitch.Normal) {
                result = 1f;
            }
            else if (pitch == Pitch.High) {
                result = 2f;
            }

            return result;
        }
    }
}