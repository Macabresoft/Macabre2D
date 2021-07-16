namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// An interface for a service which handles a scene's systems.
    /// </summary>
    public interface ISystemService : ISelectionService<IUpdateableSystem> {
    }

    /// <summary>
    /// A service which handles a scene's systems.
    /// </summary>
    public class SystemService : SelectionService<IUpdateableSystem>, ISystemService {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemService" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <param name="undoService">The undo service.</param>
        /// <param name="valueEditorService">The value editor service.</param>
        public SystemService(
            IAssemblyService assemblyService,
            IUndoService undoService,
            IValueEditorService valueEditorService) : base(assemblyService, undoService, valueEditorService) {
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetAvailableTypes(IAssemblyService assemblyService) {
            return assemblyService.LoadTypes(typeof(IUpdateableSystem));
        }
    }
}