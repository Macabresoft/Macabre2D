namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

/// <summary>
/// A view model for a type selection dialog.
/// </summary>
public class TypeSelectionViewModel : BaseFilterDialogViewModel<Type> {
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSelectionViewModel" /> class.
    /// </summary>
    /// <remarks>This constructor only exists for design time XAML.</remarks>
    public TypeSelectionViewModel() : base() {
        this.IsOkEnabled = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSelectionViewModel" /> class.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    /// <param name="defaultType">The default type.</param>
    /// <param name="title">The title of the window.</param>
    [InjectionConstructor]
    public TypeSelectionViewModel(IEnumerable<Type> types, Type defaultType, string title) : base(types) {
        this.SelectedItem = defaultType;
    }
    
    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; }

    /// <inheritdoc />
    protected override IEnumerable<Type> GetFilteredItems(string filter) => this.Items.Where(x => !string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(filter, StringComparison.OrdinalIgnoreCase));
}