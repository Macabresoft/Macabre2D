namespace Macabre2D.Engine.Windows.ServiceInterfaces {

    public interface IFileService {
        string ProjectDirectoryPath { get; }

        string GetAutoSaveDirectory();
    }
}