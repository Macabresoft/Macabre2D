namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Threading.Tasks;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    /// <summary>
    /// An interface for a dialog service.
    /// </summary>
    public interface IDialogService {
        /// <summary>
        /// Opens a dialog that allows the user to pick a <see cref="Type"/> which inherits from the provided base type.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="typesToIgnore">Types to be ignored from the types shown.</param>
        /// <returns>A type that inherits from the specified type.</returns>
        Task<Type> OpenTypeSelectionDialog(Type baseType, params Type[] typesToIgnore);

        /// <summary>
        /// Opens a dialog that allows the user to pick a <see cref="IContentNode"/> whose asset inherits from the specified base type.
        /// </summary>
        /// <param name="baseAssetType">The base asset type.</param>
        /// <param name="allowDirectorySelection">A value indicating whether or not the user can select a directory. Usually used when creating a new asset as opposed to loading one.</param>
        /// <returns>The selected content node.</returns>
        Task<IContentNode> OpenAssetSelectionDialog(Type baseAssetType, bool allowDirectorySelection);
    }
}