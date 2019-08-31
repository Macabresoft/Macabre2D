namespace Macabre2D.UI.Services {

    using Macabre2D.UI.ServiceInterfaces;
    using System.IO;
    using System.Reflection;

    public sealed class FileService : IFileService {

        public FileService() {
            this.ProjectDirectoryPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)), "..", "..", @"..\");
        }

        public string ProjectDirectoryPath { get; }
    }
}