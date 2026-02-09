namespace Macabre2D.Framework;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// Interface for a collection of <see cref="INameable" />.
/// </summary>
public interface INameableCollection : IReadOnlyCollection<INameable> {
    /// <summary>
    /// Gets the name of the collection.
    /// </summary>
    string Name { get; }
}

/// <summary>
/// A collection of <see cref="INameable" />.
/// </summary>
/// <typeparam name="TNameable"></typeparam>
public abstract class NameableCollection<TNameable> : ObservableCollectionExtended<TNameable>, INameableCollection, INotifyPropertyChanged where TNameable : INameable {
    /// <inheritdoc />
    public new event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="NameableCollection{TAsset}" /> class.
    /// </summary>
    protected NameableCollection() : base() {
        this.Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NameableCollection{TAsset}" /> class.
    /// </summary>
    /// <param name="items">The items.</param>
    protected NameableCollection(IEnumerable<TNameable> items) : base(items) {
        this.Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NameableCollection{TAsset}" /> class.
    /// </summary>
    /// <param name="items">The items.</param>
    protected NameableCollection(List<TNameable> items) : base(items) {
        this.Initialize();
    }

    /// <inheritdoc />
    public abstract string Name { get; }

    private void Asset_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RaiseItemsChanged();
    }

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => (IEnumerator<INameable>)this.Items.GetEnumerator();

    private void HandleAdd(IEnumerable? newItems) {
        if (newItems != null) {
            foreach (var asset in newItems.OfType<TNameable>()) {
                this.OnAdd(asset);
            }
        }
    }

    private void HandleRemove(IEnumerable? oldItems) {
        if (oldItems != null) {
            foreach (var asset in oldItems.OfType<TNameable>()) {
                this.OnRemove(asset);
            }
        }
    }

    private void Initialize() {
        if (BaseGame.IsDesignMode) {
            this.HandleAdd(this.Items);
            this.CollectionChanged += this.OnCollectionChanged;
        }
    }

    private void OnAdd(TNameable asset) {
        if (asset is INotifyPropertyChanged notifier) {
            notifier.PropertyChanged += this.Asset_PropertyChanged;
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        switch (e.Action) {
            case NotifyCollectionChangedAction.Add:
                this.HandleAdd(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                this.HandleRemove(e.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace or NotifyCollectionChangedAction.Reset:
                this.HandleRemove(e.OldItems);
                this.HandleAdd(e.NewItems);
                break;
        }

        this.RaiseItemsChanged();
    }

    private void OnRemove(TNameable asset) {
        if (asset is INotifyPropertyChanged notifier) {
            notifier.PropertyChanged -= this.Asset_PropertyChanged;
        }
    }

    private void RaiseItemsChanged() {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Items)));
    }
}