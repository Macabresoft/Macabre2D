namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Threading.Tasks;
    using Macabresoft.Macabre2D.Framework;

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
    }
}