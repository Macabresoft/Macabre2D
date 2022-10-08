namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using ReactiveUI;
using Unity;

/// <summary>
/// A view model for a type selection dialog.
/// </summary>
public class TypeSelectionViewModel : BaseDialogViewModel {
    private readonly ObservableCollectionExtended<Type> _filteredTypes = new();
    private readonly List<Type> _types = new();
    private string _filterText;
    private Type _selectedType;

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
    [InjectionConstructor]
    public TypeSelectionViewModel(IEnumerable<Type> types) : this() {
        this._types.AddRange(types.OrderBy(x => x.FullName));
        this.FilterTypes();
    }

    /// <summary>
    /// Gets the filtered types.
    /// </summary>
    public IReadOnlyCollection<Type> FilteredTypes => this._filteredTypes;

    /// <summary>
    /// Gets or sets the filter text.
    /// </summary>
    public string FilterText {
        get => this._filterText;
        set {
            if (this._filterText != value) {
                this._filterText = value;
                this.FilterTypes();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected type.
    /// </summary>
    public Type SelectedType {
        get => this._selectedType;
        set {
            this.RaiseAndSetIfChanged(ref this._selectedType, value);
            this.IsOkEnabled = this.SelectedType != null;
        }
    }


    /// <summary>
    /// Gets the available types.
    /// </summary>
    public IReadOnlyCollection<Type> Types => this._types;

    private void FilterTypes() {
        this._filteredTypes.Reset(!string.IsNullOrEmpty(this._filterText) ? this.Types.Where(x => !string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(this._filterText, StringComparison.OrdinalIgnoreCase)) : this.Types);
    }
}