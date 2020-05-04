namespace Macabre2D.UI.GameEditorLibrary.Services {

    using Macabre2D.UI.CommonLibrary.Common;
    using System.IO;
    using System.Reflection;

    public interface IFileService {
        string ProjectDirectoryPath { get; }

        string GetAutoSaveDirectory();
    }

    public sealed class FileService : IFileService {

        public FileService() {
            this.ProjectDirectoryPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)), "..", "..", "..", "..", @"Project\");
        }

        public string ProjectDirectoryPath { get; }

        public string GetAutoSaveDirectory() {
            var autoSavePath = Path.Combine(this.ProjectDirectoryPath, FileHelper.ProjectAutoSaveFolder);
            if (!Directory.Exists(autoSavePath)) {
                var directory = Directory.CreateDirectory(autoSavePath);
                directory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            return autoSavePath;
        }
    }
}