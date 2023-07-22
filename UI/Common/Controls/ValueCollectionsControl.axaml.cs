namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Macabresoft.Core;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

public partial class ValueCollectionsControl : UserControl {
    public static readonly StyledProperty<IEnumerable<ValueControlCollection>> CollectionsProperty =
        AvaloniaProperty.Register<ValueCollectionsControl, IEnumerable<ValueControlCollection>>(nameof(Collections), Enumerable.Empty<ValueControlCollection>());

    public static readonly StyledProperty<bool> IsBusyProperty =
        AvaloniaProperty.Register<ValueCollectionsControl, bool>(nameof(IsBusy));
    
    public ValueCollectionsControl() {
        this.ExpandAllCommand = ReactiveCommand.Create(() => this.SetIsExpanded(true));
        this.CollapseAllCommand = ReactiveCommand.Create(() => this.SetIsExpanded(false));

        this.InitializeComponent();
    }

    public ICommand CollapseAllCommand { get; }

    public ICommand ExpandAllCommand { get; }
    
    public IEnumerable<ValueControlCollection> Collections {
        get => this.GetValue(CollectionsProperty);
        set => this.SetValue(CollectionsProperty, value);
    }

    public bool IsBusy {
        get => this.GetValue(IsBusyProperty);
        set => this.SetValue(IsBusyProperty, value);
    }

    private void SetIsExpanded(bool isExpanded) {
        foreach (var collection in this.Collections) {
            collection.IsExpanded = isExpanded;
        }
    }
}