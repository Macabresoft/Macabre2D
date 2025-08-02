namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

/// <summary>
/// A view model for a resource selection dialog.
/// </summary>
public class ResourceSelectionViewModel : BaseFilterDialogViewModel<ResourceEntry> {
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSelectionViewModel" /> class.
    /// </summary>
    /// <remarks>This constructor only exists for design time XAML.</remarks>
    public ResourceSelectionViewModel() : base() {
        this.IsOkEnabled = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeSelectionViewModel" /> class.
    /// </summary>
    /// <param name="resources">The types to select from.</param>
    /// <param name="title">The title of the window.</param>
    [InjectionConstructor]
    public ResourceSelectionViewModel(IEnumerable<ResourceEntry> resources, string title) : base(resources) {
        this.Title = title;
    }
    
    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; }

    /// <inheritdoc />
    protected override IEnumerable<ResourceEntry> GetFilteredItems(string filter) {
        return this.Items.Where(x => !string.IsNullOrEmpty(x.Key) && (x.Key.Contains(filter, StringComparison.OrdinalIgnoreCase) || x.Value?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true));
    }
}