namespace Macabre2D.UI.Library.ServiceInterfaces {

    public interface IAutoSaveService {
        byte AutoSaveIntervalInMinutes { get; set; }

        byte NumberOfAutoSaves { get; set; }

        void Initialize(byte numberOfAutoSaves, byte autoSaveIntervalInMinutes);
    }
}