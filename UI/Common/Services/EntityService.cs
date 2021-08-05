namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using ReactiveUI;

    /// <summary>
    /// An interface for a service which handles the selection and loading of entities and their editors.
    /// </summary>
    public interface IEntityService : ISelectionService<IEntity> {
        /// <summary>
        /// Occurs when focus is requested for an entity.
        /// </summary>
        event EventHandler<IEntity> FocusRequested;

        /// <summary>
        /// Gets a command to request focus of the currently selected entity.
        /// </summary>
        ICommand RequestFocusCommand { get; }
    }

    /// <summary>
    /// A service which handles the selection and loading of entities and their editors.
    /// </summary>
    public sealed class EntityService : SelectionService<IEntity>, IEntityService {
        /// <inheritdoc />
        public event EventHandler<IEntity> FocusRequested;

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
            this.RequestFocusCommand = ReactiveCommand.Create(
                this.RequestFocus,
                this.WhenAny(x => x.Selected, y => y.Value != null));
        }

        /// <inheritdoc />
        public ICommand RequestFocusCommand { get; }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetAvailableTypes(IAssemblyService assemblyService) {
            return assemblyService.LoadTypes(typeof(IEntity)).Where(x => !x.IsAssignableTo(typeof(IScene)));
        }

        private void RequestFocus() {
            this.FocusRequested.SafeInvoke(this, this.Selected);
        }
    }
}