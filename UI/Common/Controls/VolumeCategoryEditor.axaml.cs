namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Controls;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Project.Common;

public partial class VolumeCategoryEditor : UserControl, IObserver<AvaloniaPropertyChangedEventArgs<VolumeCategory>> {
    public static readonly StyledProperty<VolumeCategory> CategoryProperty =
        AvaloniaProperty.Register<VolumeCategoryEditor, VolumeCategory>(nameof(Category));

    public static readonly DirectProperty<VolumeCategoryEditor, float> VolumeProperty =
        AvaloniaProperty.RegisterDirect<VolumeCategoryEditor, float>(
            nameof(Volume),
            editor => editor.Volume,
            (editor, value) => editor.Volume = value);

    private readonly IProjectService _projectService;
    private readonly IUndoService _undoService;

    public VolumeCategoryEditor() : this(Resolver.Resolve<IProjectService>(), Resolver.Resolve<IUndoService>()) {
    }

    public VolumeCategoryEditor(IProjectService projectService, IUndoService undoService) {
        this._projectService = projectService;
        this._undoService = undoService;
        CategoryProperty.Changed.Subscribe(this);
        this.InitializeComponent();
    }

    public VolumeCategory Category {
        get => this.GetValue(CategoryProperty);
        set => this.SetValue(CategoryProperty, value);
    }

    public float Volume {
        get => this._projectService.CurrentProject.DefaultUserSettings.Audio.GetCategoryVolume(this.Category);
        set {
            var originalVolume = this.Volume;
            this._undoService.Do(
                () =>
                {
                    this._projectService.CurrentProject.DefaultUserSettings.Audio.SetCategoryVolume(this.Category, value);
                    this.RaisePropertyChanged(VolumeProperty, originalVolume, this.Volume);
                }, () =>
                {
                    this._projectService.CurrentProject.DefaultUserSettings.Audio.SetCategoryVolume(this.Category, originalVolume);
                    this.RaisePropertyChanged(VolumeProperty, value, this.Volume);
                });
        }
    }

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<VolumeCategory> value) {
        var oldValue = this._projectService.CurrentProject.DefaultUserSettings.Audio.GetCategoryVolume(value.OldValue.Value);
        var newValue = this._projectService.CurrentProject.DefaultUserSettings.Audio.GetCategoryVolume(value.NewValue.Value);
        this.RaisePropertyChanged(VolumeProperty, oldValue, newValue);
    }
}