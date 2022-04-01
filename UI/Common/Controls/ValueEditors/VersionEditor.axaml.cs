namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Unity;

public class VersionEditor : ValueEditorControl<Version> {
    public static readonly DirectProperty<VersionEditor, int> BuildValueProperty =
        AvaloniaProperty.RegisterDirect<VersionEditor, int>(
            nameof(BuildValue),
            editor => editor.BuildValue,
            (editor, value) => editor.BuildValue = value);

    public static readonly DirectProperty<VersionEditor, int> MajorValueProperty =
        AvaloniaProperty.RegisterDirect<VersionEditor, int>(
            nameof(MajorValue),
            editor => editor.MajorValue,
            (editor, value) => editor.MajorValue = value);

    public static readonly DirectProperty<VersionEditor, int> MinorValueProperty =
        AvaloniaProperty.RegisterDirect<VersionEditor, int>(
            nameof(MinorValue),
            editor => editor.MinorValue,
            (editor, value) => editor.MinorValue = value);

    public static readonly DirectProperty<VersionEditor, int> RevisionValueProperty =
        AvaloniaProperty.RegisterDirect<VersionEditor, int>(
            nameof(RevisionValue),
            editor => editor.RevisionValue,
            (editor, value) => editor.RevisionValue = value);

    private int _buildValue;
    private int _majorValue;
    private int _minorValue;
    private int _revisionValue;

    public VersionEditor() : this(null) {
    }

    [InjectionConstructor]
    public VersionEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    public int BuildValue {
        get => Math.Max(0, this._buildValue);
        set => this.SetAndRaise(BuildValueProperty, ref this._buildValue, value);
    }

    public int MajorValue {
        get => Math.Max(0, this._majorValue);
        set => this.SetAndRaise(MajorValueProperty, ref this._majorValue, value);
    }

    public int MinorValue {
        get => Math.Max(0, this._minorValue);
        set => this.SetAndRaise(MinorValueProperty, ref this._minorValue, value);
    }

    public int RevisionValue {
        get => Math.Max(0, this._revisionValue);
        set => this.SetAndRaise(RevisionValueProperty, ref this._revisionValue, value);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.UpdateDisplayValues();
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(this.Value)) {
            this.UpdateDisplayValues();
        }
        else if (change.Property.Name is nameof(this.MajorValue) or nameof(this.MinorValue) or nameof(this.BuildValue) or nameof(this.RevisionValue)) {
            Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Version(this.MajorValue, this.MinorValue, this.BuildValue, this.RevisionValue)));
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateDisplayValues() {
        if (this.Value != null) {
            this._majorValue = this.Value.Major;
            this._minorValue = this.Value.Minor;
            this._buildValue = this.Value.Build;
            this._revisionValue = this.Value.Revision;
            Dispatcher.UIThread.Post(() => {
                this.RaisePropertyChanged(MajorValueProperty, Optional<int>.Empty, this.MajorValue);
                this.RaisePropertyChanged(MinorValueProperty, Optional<int>.Empty, this.MinorValue);
                this.RaisePropertyChanged(BuildValueProperty, Optional<int>.Empty, this.BuildValue);
                this.RaisePropertyChanged(RevisionValueProperty, Optional<int>.Empty, this.RevisionValue);
            });
        }
    }
}