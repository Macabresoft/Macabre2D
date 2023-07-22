namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;

public partial class AudioSettingsEditor : ValueEditorControl<AudioSettings> {
    public static readonly DirectProperty<AudioSettingsEditor, float> EffectVolumeProperty =
        AvaloniaProperty.RegisterDirect<AudioSettingsEditor, float>(
            nameof(EffectVolume),
            editor => editor.EffectVolume,
            (editor, value) => editor.EffectVolume = value);

    public static readonly DirectProperty<AudioSettingsEditor, float> MenuVolumeProperty =
        AvaloniaProperty.RegisterDirect<AudioSettingsEditor, float>(
            nameof(MenuVolume),
            editor => editor.MenuVolume,
            (editor, value) => editor.MenuVolume = value);

    public static readonly DirectProperty<AudioSettingsEditor, float> MusicVolumeProperty =
        AvaloniaProperty.RegisterDirect<AudioSettingsEditor, float>(
            nameof(MusicVolume),
            editor => editor.MusicVolume,
            (editor, value) => editor.MusicVolume = value);

    public static readonly DirectProperty<AudioSettingsEditor, float> NotificationVolumeProperty =
        AvaloniaProperty.RegisterDirect<AudioSettingsEditor, float>(
            nameof(NotificationVolume),
            editor => editor.NotificationVolume,
            (editor, value) => editor.NotificationVolume = value);

    public static readonly DirectProperty<AudioSettingsEditor, float> OverallVolumeProperty =
        AvaloniaProperty.RegisterDirect<AudioSettingsEditor, float>(
            nameof(OverallVolume),
            editor => editor.OverallVolume,
            (editor, value) => editor.OverallVolume = value);

    public static readonly DirectProperty<AudioSettingsEditor, float> VoiceVolumeProperty =
        AvaloniaProperty.RegisterDirect<AudioSettingsEditor, float>(
            nameof(VoiceVolume),
            editor => editor.VoiceVolume,
            (editor, value) => editor.VoiceVolume = value);

    private readonly IUndoService _undoService;

    public AudioSettingsEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public AudioSettingsEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.InitializeComponent();
    }

    public float EffectVolume {
        get => this.Value.EffectVolume;
        set {
            var originalVolume = this.Value.EffectVolume;
            this._undoService.Do(
                () =>
                {
                    this.Value.EffectVolume = value;
                    this.RaisePropertyChanged(EffectVolumeProperty, originalVolume, this.EffectVolume);
                }, () =>
                {
                    this.Value.EffectVolume = originalVolume;
                    this.RaisePropertyChanged(EffectVolumeProperty, value, this.EffectVolume);
                });
        }
    }

    public float MenuVolume {
        get => this.Value.MenuVolume;
        set {
            var originalVolume = this.Value.MenuVolume;
            this._undoService.Do(
                () =>
                {
                    this.Value.MenuVolume = value;
                    this.RaisePropertyChanged(MenuVolumeProperty, originalVolume, this.MenuVolume);
                }, () =>
                {
                    this.Value.MenuVolume = originalVolume;
                    this.RaisePropertyChanged(MenuVolumeProperty, value, this.MenuVolume);
                });
        }
    }

    public float MusicVolume {
        get => this.Value.MusicVolume;
        set {
            var originalVolume = this.Value.MusicVolume;
            this._undoService.Do(
                () =>
                {
                    this.Value.MusicVolume = value;
                    this.RaisePropertyChanged(MusicVolumeProperty, originalVolume, this.MusicVolume);
                }, () =>
                {
                    this.Value.MusicVolume = originalVolume;
                    this.RaisePropertyChanged(MusicVolumeProperty, value, this.MusicVolume);
                });
        }
    }

    public float NotificationVolume {
        get => this.Value.NotificationVolume;
        set {
            var originalVolume = this.Value.NotificationVolume;
            this._undoService.Do(
                () =>
                {
                    this.Value.NotificationVolume = value;
                    this.RaisePropertyChanged(NotificationVolumeProperty, originalVolume, this.NotificationVolume);
                }, () =>
                {
                    this.Value.NotificationVolume = originalVolume;
                    this.RaisePropertyChanged(NotificationVolumeProperty, value, this.NotificationVolume);
                });
        }
    }

    public float OverallVolume {
        get => this.Value.OverallVolume;
        set {
            var originalVolume = this.Value.OverallVolume;
            this._undoService.Do(
                () =>
                {
                    this.Value.OverallVolume = value;
                    this.RaisePropertyChanged(OverallVolumeProperty, originalVolume, this.OverallVolume);
                }, () =>
                {
                    this.Value.OverallVolume = originalVolume;
                    this.RaisePropertyChanged(OverallVolumeProperty, value, this.OverallVolume);
                });
        }
    }

    public float VoiceVolume {
        get => this.Value.VoiceVolume;
        set {
            var originalVolume = this.Value.VoiceVolume;
            this._undoService.Do(
                () =>
                {
                    this.Value.VoiceVolume = value;
                    this.RaisePropertyChanged(VoiceVolumeProperty, originalVolume, this.VoiceVolume);
                }, () =>
                {
                    this.Value.OverallVolume = originalVolume;
                    this.RaisePropertyChanged(VoiceVolumeProperty, value, this.VoiceVolume);
                });
        }
    }
}