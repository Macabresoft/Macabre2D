using System.ComponentModel;

namespace Macabre2D.UI.Library.ServiceInterfaces {

    public interface IStatusService : INotifyPropertyChanged {
        float PrimaryGridSize { get; set; }

        float SecondaryGridSize { get; set; }

        float ViewHeight { get; set; }

        float ViewWidth { get; set; }
    }
}