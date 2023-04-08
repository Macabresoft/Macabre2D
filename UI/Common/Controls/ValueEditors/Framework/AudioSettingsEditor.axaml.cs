namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class AudioSettingsEditor : ValueEditorControl<AudioSettings> {
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
    private IDisposable _effectDisposable;
    private IDisposable _menuDisposable;
    private IDisposable _musicDisposable;
    private IDisposable _notificationDisposable;
    private IDisposable _overallDisposable;
    private IDisposable _voiceDisposable;

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        var sliders = this.GetLogicalDescendants().OfType<Slider>();
        foreach (var slider in sliders) {
            if (slider.Tag is AudioCategory category) {
                if (category == AudioCategory.Default) {
                    this._overallDisposable?.Dispose();
                    this._overallDisposable = slider.AddDisposableHandler(PointerReleasedEvent, this.OverallVolume_PointerReleased, RoutingStrategies.Tunnel);
                }
                else if (category == AudioCategory.Effect) {
                    this._effectDisposable?.Dispose();
                    this._effectDisposable = slider.AddDisposableHandler(PointerReleasedEvent, this.EffectVolume_PointerReleased, RoutingStrategies.Tunnel);
                }
                else if (category == AudioCategory.Menu) {
                    this._menuDisposable?.Dispose();
                    this._menuDisposable = slider.AddDisposableHandler(PointerReleasedEvent, this.MenuVolume_PointerReleased, RoutingStrategies.Tunnel);
                }
                else if (category == AudioCategory.Music) {
                    this._musicDisposable?.Dispose();
                    this._musicDisposable = slider.AddDisposableHandler(PointerReleasedEvent, this.MusicVolume_PointerReleased, RoutingStrategies.Tunnel);
                }
                else if (category == AudioCategory.Notification) {
                    this._notificationDisposable?.Dispose();
                    this._notificationDisposable = slider.AddDisposableHandler(PointerReleasedEvent, this.NotificationVolume_PointerReleased, RoutingStrategies.Tunnel);
                }
                else if (category == AudioCategory.Voice) {
                    this._voiceDisposable?.Dispose();
                    this._voiceDisposable = slider.AddDisposableHandler(PointerReleasedEvent, this.VoiceVolume_PointerReleased, RoutingStrategies.Tunnel);
                }
            }
        }
    }

    private void EffectVolume_PointerReleased(object sender, PointerReleasedEventArgs e) {
        if (sender is Slider slider) {
            var value = (float)slider.Value;
            if (Math.Abs(value - this.EffectVolume) > 0.001f) {
                this.EffectVolume = (float)slider.Value;
            }
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void MenuVolume_PointerReleased(object sender, PointerReleasedEventArgs e) {
        if (sender is Slider slider) {
            var value = (float)slider.Value;
            if (Math.Abs(value - this.MenuVolume) > 0.001f) {
                this.MenuVolume = (float)slider.Value;
            }
        }
    }

    private void MusicVolume_PointerReleased(object sender, PointerReleasedEventArgs e) {
        if (sender is Slider slider) {
            var value = (float)slider.Value;
            if (Math.Abs(value - this.MusicVolume) > 0.001f) {
                this.MusicVolume = (float)slider.Value;
            }
        }
    }

    private void NotificationVolume_PointerReleased(object sender, PointerReleasedEventArgs e) {
        if (sender is Slider slider) {
            var value = (float)slider.Value;
            if (Math.Abs(value - this.NotificationVolume) > 0.001f) {
                this.NotificationVolume = (float)slider.Value;
            }
        }
    }

    private void OverallVolume_PointerReleased(object sender, PointerReleasedEventArgs e) {
        if (sender is Slider slider) {
            var value = (float)slider.Value;
            if (Math.Abs(value - this.OverallVolume) > 0.001f) {
                this.OverallVolume = (float)slider.Value;
            }
        }
    }

    private void VoiceVolume_PointerReleased(object sender, PointerReleasedEventArgs e) {
        if (sender is Slider slider) {
            var value = (float)slider.Value;
            if (Math.Abs(value - this.VoiceVolume) > 0.001f) {
                this.VoiceVolume = (float)slider.Value;
            }
        }
    }
}