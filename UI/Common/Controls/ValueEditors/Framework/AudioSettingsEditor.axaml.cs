namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using Avalonia;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Unity;

public partial class AudioSettingsEditor : ValueEditorControl<AudioSettings> {
    public static readonly DirectProperty<AudioSettingsEditor, IReadOnlyCollection<VolumeCategory>> CategoriesProperty =
        AvaloniaProperty.RegisterDirect<AudioSettingsEditor, IReadOnlyCollection<VolumeCategory>>(
            nameof(Categories),
            editor => editor.Categories);


    public AudioSettingsEditor() : this(null) {
    }

    [InjectionConstructor]
    public AudioSettingsEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.Categories = Enum.GetValues<VolumeCategory>();
        this.InitializeComponent();
    }

    public IReadOnlyCollection<VolumeCategory> Categories { get; }
}