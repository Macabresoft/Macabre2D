namespace Macabresoft.Macabre2D.UI.Common {
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
        /// <param name="valueControlService">The value editor service.</param>
        public SystemService(
            IAssemblyService assemblyService,
            IUndoService undoService,
            IValueControlService valueControlService) : base(assemblyService, undoService, valueControlService) {
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetAvailableTypes(IAssemblyService assemblyService) {
            return assemblyService.LoadTypes(typeof(IUpdateableSystem));
        }
    }
}