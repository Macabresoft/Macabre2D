namespace Macabresoft.Macabre2D.Editor.Library.Models {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Macabresoft.Macabre2D.Editor.Library.Services;

    /// <summary>
    /// A <see cref="IValueEditor" /> that maintains its own child value editors.
    /// </summary>
    public interface IParentValueEditor : IValueEditor {
        /// <summary>
        /// The child editors.
        /// </summary>
        IReadOnlyCollection<IValueEditor> ChildEditors { get; }

        /// <summary>
        /// Initializes this instance with a value editor service and assembly service.
        /// </summary>
        /// <param name="valueEditorService"></param>
        /// <param name="assemblyService"></param>
        void Initialize(IValueEditorService valueEditorService, IAssemblyService assemblyService);
    }
}