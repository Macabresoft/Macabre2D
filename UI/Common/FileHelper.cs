using System.IO;
using System.Threading.Tasks;

namespace Macabre2D.UI.Common {

    public static class FileHelper {
        public const string AssetManagerExtension = ".m2dam";
        public const string AutoTileSetExtension = ".m2dautotile";
        public const string BackupExtension = ".backup";
        public const string ContentExtension = ".mgcb";
        public const string GameSettingsExtension = ".m2dgs";
        public const string MetaDataExtension = ".m2dmetadata";
        public const string MonoGameFrameworkDLL = "MonoGame.Framework.dll";
        public const string NewFolderDefaultName = "New Folder";
        public const string PrefabExtension = ".m2dprefab";
        public const string ProjectExtension = ".m2dproj";
        public const string SceneExtension = ".m2dscene";
        public const string SolutionExtension = ".sln";
        public const string SpriteAnimationExtension = ".m2dspranim";
        public const string SpriteFontExtension = ".spritefont";
        public const string TileSetExtension = ".m2dtileset";
        public static string[] AudioExtensions = { ".mp3", ".wav", ".wma" };
        public static string[] ImageExtensions = { ".png", ".jpg", ".gif" };

        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target) {
            Directory.CreateDirectory(target.FullName);

            foreach (var file in source.EnumerateFiles()) {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (var directory in source.EnumerateDirectories()) {
                var nextTarget = target.CreateSubdirectory(directory.Name);
                CopyDirectory(directory, nextTarget);
            }
        }

        public static void CopyDirectory(string sourceDirectory, string targetDirectory) {
            var sourceInfo = new DirectoryInfo(sourceDirectory);
            var targetInfo = new DirectoryInfo(targetDirectory);

            CopyDirectory(sourceInfo, targetInfo);
        }

        public static async Task DeleteDirectory(string directoryPath, short secondsToAttempt, bool recursive) {
            if (Directory.Exists(directoryPath)) {
                short count = 0;
                var wasSuccessful = false;
                while (!wasSuccessful && count < secondsToAttempt) {
                    try {
                        Directory.Delete(directoryPath, true);
                        wasSuccessful = true;
                    }
                    catch (IOException e) {
                        wasSuccessful = false;
                        await Task.Delay(1000);
                        count++;

                        if (count >= secondsToAttempt) {
                            throw e;
                        }
                    }
                }
            }
        }

        public static bool IsAudioFile(string fileName) {
            var upperFileName = fileName.ToLower();
            foreach (var imageExtension in FileHelper.AudioExtensions) {
                if (upperFileName.EndsWith(imageExtension)) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsImageFile(string fileName) {
            var upperFileName = fileName.ToLower();
            foreach (var imageExtension in FileHelper.ImageExtensions) {
                if (upperFileName.EndsWith(imageExtension)) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsMetadataFile(string fileName) {
            return fileName.ToUpper().EndsWith(FileHelper.MetaDataExtension.ToUpper());
        }

        public static bool IsValidFileName(string fileName) {
            return !string.IsNullOrEmpty(fileName) && fileName.IndexOfAny(Path.GetInvalidPathChars()) < 0;
        }

        public static string WithoutExtension(this string fileOrFolderName) {
            var result = Path.GetFileNameWithoutExtension(fileOrFolderName);
            if (string.IsNullOrWhiteSpace(result)) {
                result = fileOrFolderName;
            }

            return result;
        }
    }
}