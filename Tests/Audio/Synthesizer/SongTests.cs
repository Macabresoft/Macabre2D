namespace Macabre2D.Framework.Tests.Audio.Synthesizer {

    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Linq;

    [TestFixture]
    public static class SongTests {

        [Test]
        [Category("Unit Test")]
        public static void Song_SerializerTest() {
            var random = new Random();
            var song = new Song();

            for (var i = 0; i < random.Next(1, 5); i++) {
                var track = song.AddTrack();
                track.LeftChannelVolume = (float)random.NextDouble();
                track.RightChannelVolume = (float)random.NextDouble();
            }

            song.AddTrack();
            song.BeatsPerMinute = (ushort)random.Next(Song.MinimumBeatsPerMinute, Song.MaximumBeatsPerMinute);
            song.SampleRate = (ushort)random.Next(Song.MinimumSampleRate, Song.MaximumSampleRate);

            var fileLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, "SongForTest.cosmicjam");
            Song deserializedSong = null;
            var serializer = new Serializer();

            try {
                serializer.Serialize(song, fileLocation);
                deserializedSong = serializer.Deserialize<Song>(fileLocation);
            }
            finally {
                File.Delete(fileLocation);
            }

            CompareSongs(song, deserializedSong);
        }

        private static void CompareSongs(Song originalSong, Song deserializedSong) {
            Assert.AreEqual(originalSong.BeatsPerMinute, deserializedSong.BeatsPerMinute);
            Assert.AreEqual(originalSong.SampleRate, deserializedSong.SampleRate);
            Assert.AreEqual(originalSong.Tracks.Count, deserializedSong.Tracks.Count);

            for (var i = 0; i < originalSong.Tracks.Count; i++) {
                CompareTracks(originalSong.Tracks.ElementAt(i), deserializedSong.Tracks.ElementAt(i));
            }
        }

        private static void CompareTracks(Track originalTrack, Track deserializedTrack) {
            Assert.AreEqual(originalTrack.LeftChannelVolume, deserializedTrack.LeftChannelVolume);
            Assert.AreEqual(originalTrack.RightChannelVolume, deserializedTrack.RightChannelVolume);
        }
    }
}