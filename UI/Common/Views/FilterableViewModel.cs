namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabre2D.Framework;
using ReactiveUI;

public abstract class FilterableViewModel<TFiltered> : BaseDialogViewModel where TFiltered : class, INameable {
    private readonly ObservableCollectionExtended<TFiltered> _filteredNodes = [];
    private readonly List<TFiltered> _nodesAvailableToFilter = [];

    private TFiltered _filteredSelection;
    private string _filterText;

    /// <summary>
    /// Constructs a new instance of the <see cref="FilterableViewModel{TFiltered}" /> class.
    /// </summary>
    /// <param name="undoService">The undo service.</param>
    protected FilterableViewModel(IUndoService undoService) : base(undoService) {
        this.ClearFilterCommand = ReactiveCommand.Create(this.ClearFilter, this.WhenAny(x => x.IsFiltered, _ => this.IsFiltered));
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="FilterableViewModel{TFiltered}" /> class.
    /// </summary>
    protected FilterableViewModel() : base() {
        this.ClearFilterCommand = ReactiveCommand.Create(this.ClearFilter, this.WhenAny(x => x.IsFiltered, _ => this.IsFiltered));
    }


    /// <summary>
    /// Gets a command to clear the filter.
    /// </summary>
    public ICommand ClearFilterCommand { get; }

    /// <summary>
    /// Gets the filtered nodes.
    /// </summary>
    public IReadOnlyCollection<TFiltered> FilteredNodes => this._filteredNodes;

    /// <summary>
    /// Gets or sets the filtered selection;
    /// </summary>
    public TFiltered FilteredSelection {
        get => this._filteredSelection;
        set {
            if (this.IsFiltered) {
                this.RaiseAndSetIfChanged(ref this._filteredSelection, value);
                this.SetActualSelected(this.FilteredSelection);
            }
            else {
                this._filteredSelection = null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the filter text.
    /// </summary>
    public string FilterText {
        get => this._filterText;
        set {
            if (this._filterText != value) {
                if (string.IsNullOrEmpty(this._filterText)) {
                    this.RefreshAvailableNodes();
                }

                this._filterText = value;
                this.PerformFilter();
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.IsFiltered));
                this.OnFilterChanged();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this is filtered.
    /// </summary>
    public bool IsFiltered => !string.IsNullOrEmpty(this.FilterText);

    /// <summary>
    /// Gets the object that is selected outside the filter.
    /// </summary>
    /// <returns>The selected object.</returns>
    protected abstract TFiltered GetActualSelected();

    /// <summary>
    /// Gets the nodes available to filter.
    /// </summary>
    /// <returns>The nodes available to filter.</returns>
    protected abstract IEnumerable<TFiltered> GetNodesAvailableToFilter();

    /// <summary>
    /// Called when the filter changes.
    /// </summary>
    protected virtual void OnFilterChanged() {
    }

    /// <summary>
    /// Sets the selection outside the filter context.
    /// </summary>
    /// <param name="selected">The selected value.</param>
    protected abstract void SetActualSelected(TFiltered selected);

    private void ClearFilter() {
        var selected = this.GetActualSelected();
        this.SetActualSelected(null);
        this.FilterText = null;
        this.SetActualSelected(selected);
    }

    private void PerformFilter() {
        if (!string.IsNullOrEmpty(this._filterText) && this._nodesAvailableToFilter.Count != 0) {
            this._filteredNodes.Reset(this._nodesAvailableToFilter.Where(x =>
                !string.IsNullOrEmpty(x.Name) && x.Name.Contains(this._filterText, StringComparison.OrdinalIgnoreCase) ||
                x.GetType().Name.Contains(this._filterText, StringComparison.OrdinalIgnoreCase)));
        }
        else {
            this._filteredNodes.Clear();
        }
    }

    private void RefreshAvailableNodes() {
        this._filteredSelection = null;
        this.RaisePropertyChanged(nameof(this.FilteredSelection));
        this._nodesAvailableToFilter.Clear();
        this._nodesAvailableToFilter.AddRange(this.GetNodesAvailableToFilter());
    }
}