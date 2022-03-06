namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// An interface for a service which handles a scene's loops.
/// </summary>
public interface ILoopService : ISelectionService<ILoop> {
    /// <summary>
    /// Gets the available entity types.
    /// </summary>
    IReadOnlyCollection<Type> AvailableTypes { get; }
}

/// <summary>
/// A service which handles a scene's loops.
/// </summary>
public class LoopService : SelectionService<ILoop>, ILoopService {
    /// <summary>
    /// Initializes a new instance of the <see cref="LoopService" /> class.
    /// </summary>
    /// <param name="assemblyService">The assembly service.</param>
    /// <param name="undoService">The undo service.</param>
    /// <param name="valueControlService">The value editor service.</param>
    public LoopService(
        IAssemblyService assemblyService,
        IUndoService undoService,
        IValueControlService valueControlService) : base(assemblyService, undoService, valueControlService) {
        this.AvailableTypes = this.AssemblyService.LoadTypes(typeof(ILoop)).ToList();
        this.AvailableTypes = this.AssemblyService.LoadTypes(typeof(ILoop)).Where(x =>
            x.Assembly != typeof(IGizmo).Assembly &&
            x.GetConstructor(Type.EmptyTypes) != null).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<Type> AvailableTypes { get; }
}