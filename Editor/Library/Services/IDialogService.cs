namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// An interface for a dialog service.
    /// </summary>
    public interface IDialogService {
        /// <summary>
        /// Opens a dialog that allows the user to pick a <see cref="Type"/> which inherits from the provided base type.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <returns>A type that inherits from <see cref="IGameComponent"/>.</returns>
        Type OpenTypeSelectionDialog(Type baseType);
    }
}