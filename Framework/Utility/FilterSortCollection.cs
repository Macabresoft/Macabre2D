namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// The FilterSortCollection class provides efficient, reusable sorting and filtering based on a
/// configurable sort comparer, filter predicate, and associate change events. I stole all of
/// this code from the MonoGame source code. Theirs is private to Game, I wanted to reuse this
/// all over the place.
/// </summary>
public class FilterSortCollection<T> : FilterCollection<T> where T : INotifyPropertyChanged {
    private readonly Comparison<AddJournalEntry> _addJournalSortComparison;
    private readonly Comparison<int> _removeJournalSortComparison = (x, y) => Comparer<int>.Default.Compare(y, x);
    private readonly Comparison<T> _sort;
    private readonly string _sortPropertyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterSortCollection{T}" /> class.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="filterPropertyName">Name of the filter property.</param>
    /// <param name="sort">The sort.</param>
    /// <param name="sortPropertyName">Name of the sort property.</param>
    public FilterSortCollection(
        Predicate<T> filter,
        string filterPropertyName,
        Comparison<T> sort,
        string sortPropertyName) : base(filter, filterPropertyName) {
        this._sort = sort;
        this._sortPropertyName = sortPropertyName;

        this._addJournalSortComparison = this.CompareAddJournalEntry;
    }

    /// <inheritdoc />
    protected override void OnItemPropertyChanged(T item, string propertyName) {
        base.OnItemPropertyChanged(item, propertyName);

        if (propertyName == this._sortPropertyName) {
            var index = this.Items.IndexOf(item);

            this.AddJournal.Add(new AddJournalEntry(this.AddJournal.Count, item));
            this.RemoveJournal.Add(index);

            // Until the item is back in place, we don't care about its events. We will
            // re-subscribe when this._addJournal is processed.
            this.UnsubscribeFromItemEvents(item);
            this.ShouldRebuildCache = true;
        }
    }

    /// <inheritdoc />
    protected override void ProcessAddJournal() {
        if (this.AddJournal.Count == 0) {
            return;
        }

        // Prepare the this._addJournal to be merge-sorted with this._items. this._items is
        // already sorted (because it is always sorted).
        this.AddJournal.Sort(this._addJournalSortComparison);

        var iAddJournal = 0;
        var iItems = 0;

        while (iItems < this.Items.Count && iAddJournal < this.AddJournal.Count) {
            var addJournalItem = this.AddJournal[iAddJournal].Item;
            // If addJournalItem is less than (belongs before) this._items[iItems], insert it.
            if (this._sort(addJournalItem, this.Items[iItems]) < 0) {
                this.Items.Insert(iItems, addJournalItem);
                this.SubscribeToItemEvents(addJournalItem);
                iAddJournal++;
            }

            // Always increment iItems, either because we inserted and need to move past the
            // insertion, or because we didn't insert and need to consider the next element.
            iItems++;
        }

        // If this._addJournal had any "tail" items, append them all now.
        for (; iAddJournal < this.AddJournal.Count; iAddJournal++) {
            var addJournalItem = this.AddJournal[iAddJournal].Item;
            this.Items.Add(addJournalItem);
            this.SubscribeToItemEvents(addJournalItem);
        }

        this.AddJournal.Clear();
    }

    /// <inheritdoc />
    protected override void ProcessRemoveJournal() {
        if (this.RemoveJournal.Count == 0) {
            return;
        }

        // Remove items in reverse. (Technically there exist faster ways to bulk-remove from a
        // variable-length array, but List<T> does not provide such a method.)
        this.RemoveJournal.Sort(this._removeJournalSortComparison);
        foreach (var t in this.RemoveJournal) {
            var index = t;
            if (index < this.Items.Count) {
                this.Items.RemoveAt(t);
            }
        }

        this.RemoveJournal.Clear();
    }

    private int CompareAddJournalEntry(AddJournalEntry x, AddJournalEntry y) {
        var result = this._sort(x.Item, y.Item);
        if (result != 0) {
            return result;
        }

        return x.Order - y.Order;
    }
}