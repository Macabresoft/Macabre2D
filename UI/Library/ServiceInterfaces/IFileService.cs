namespace Macabre2D.UI.Library.ServiceInterfaces {

    public interface IFileService {
        string ProjectDirectoryPath { get; }

        string GetAutoSaveDirectory();
    }
}