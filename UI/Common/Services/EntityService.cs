namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// An interface for a service which handles the selection and loading of entities and their editors.
    /// </summary>
    public interface IEntityService : ISelectionService<IEntity> {
    }

    /// <summary>
    /// A service which handles the selection and loading of entities and their editors.
    /// </summary>
    public sealed class EntityService : SelectionService<IEntity>, IEntityService {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityService" /> class.
        /// </summary>
        /// <param name="assemblyService">The assembly service.</param>
        /// <param name="undoService">The undo service.</param>
        /// <param name="valueEditorService">The value editor service.</param>
        public EntityService(
            IAssemblyService assemblyService,
            IUndoService undoService,
            IValueEditorService valueEditorService) : base(assemblyService, undoService, valueEditorService) {
        }
        
        /// <inheritdoc />
        protected override IEnumerable<Type> GetAvailableTypes(IAssemblyService assemblyService) {
            return assemblyService.LoadTypes(typeof(IEntity)).Where(x => !x.IsAssignableTo(typeof(IScene)));
        }
    }
}