namespace Macabresoft.Macabre2D.UI.Common; 

/// <summary>
/// Interface for a popup to be created by the popup service.
/// </summary>
public interface IPopup {
    /// <summary>
    /// Gets the header.
    /// </summary>
    string Header { get; }

    /// <summary>
    /// Tries to close the popup.
    /// </summary>
    /// <returns></returns>
    bool TryClose();
}