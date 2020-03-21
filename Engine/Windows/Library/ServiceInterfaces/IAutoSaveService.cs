namespace Macabre2D.Engine.Windows.ServiceInterfaces {

    public interface IAutoSaveService {
        byte AutoSaveIntervalInMinutes { get; set; }

        byte NumberOfAutoSaves { get; set; }

        void Initialize(byte numberOfAutoSaves, byte autoSaveIntervalInMinutes);
    }
}