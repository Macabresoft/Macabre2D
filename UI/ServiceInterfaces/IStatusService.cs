namespace Macabre2D.UI.ServiceInterfaces {

    public interface IStatusService {
        string CurrentStatusMessage { get; set; }

        float PrimaryGridSize { get; }

        float SecondaryGridSize { get; }

        float ViewHeight { get; }

        float ViewWidth { get; }
    }
}