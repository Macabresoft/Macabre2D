namespace Macabre2D.Engine.Windows.Services {
    using Macabre2D.Engine.Windows.ServiceInterfaces;
    using System.IO;
    using System.Reflection;

    public sealed class FileService : IFileService {
        public FileService() {
            this.ProjectDirectoryPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)), "..", "..", "..", "..", "..", @"Project\");
        }

        public string ProjectDirectoryPath { get; }
    }
}