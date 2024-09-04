namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using ReactiveUI;

/// <summary>
/// Base class for simple filtering dialogs.
/// </summary>
/// <typeparam name="T">The type being filtered.</typeparam>
public abstract class BaseFilterDialogViewModel<T> : BaseDialogViewModel {
    private readonly ObservableCollectionExtended<T> _filteredItems = new();
    private readonly List<T> _items = [];
    private string _filterText;
    private T _selectedItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFilterDialogViewModel{T}" /> class.
    /// </summary>
    /// <remarks>This constructor only exists for design time XAML.</remarks>
    protected BaseFilterDialogViewModel() : base() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFilterDialogViewModel{T}" /> class.
    /// </summary>
    /// <param name="items">The items to select from.</param>
    protected BaseFilterDialogViewModel(IEnumerable<T> items) : this() {
        this._items.AddRange(items);
        this.FilterItems();
    }

    /// <summary>
    /// Gets the filtered resources.
    /// </summary>
    public IReadOnlyCollection<T> FilteredItems => this._filteredItems;


    /// <summary>
    /// Gets the available resources.
    /// </summary>
    public IReadOnlyCollection<T> Items => this._items;

    /// <summary>
    /// Gets or sets the filter text.
    /// </summary>
    public string FilterText {
        get => this._filterText;
        set {
            if (this._filterText != value) {
                this._filterText = value;
                this.FilterItems();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    public T SelectedItem {
        get => this._selectedItem;
        set {
            this.RaiseAndSetIfChanged(ref this._selectedItem, value);
            this.IsOkEnabled = this.SelectedItem != null;
        }
    }

    /// <summary>
    /// Filters the items.
    /// </summary>
    protected void FilterItems() {
        this._filteredItems.Reset(!string.IsNullOrEmpty(this._filterText) ? this.GetFilteredItems(this._filterText) : this.Items);
        if (this.SelectedItem == null || !this._filteredItems.Contains(this.SelectedItem)) {
            this.SelectedItem = this._filteredItems.FirstOrDefault();
        }
    }

    /// <summary>
    /// Gets the filtered items.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The filtered item.</returns>
    protected abstract IEnumerable<T> GetFilteredItems(string filter);
}