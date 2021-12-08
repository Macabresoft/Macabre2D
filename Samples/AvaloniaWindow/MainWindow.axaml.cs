namespace Macabresoft.Macabre2D.Samples.AvaloniaWindow;

using System.Windows.Input;
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.UI.AvaloniaInterop;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;

public class MainWindow : BaseDialog {
    public static readonly DirectProperty<MainWindow, bool> ShowHintProperty =
        AvaloniaProperty.RegisterDirect<MainWindow, bool>(
            nameof(ShowHint),
            editor => editor.ShowHint);

    public static readonly DirectProperty<MainWindow, bool> ShowSkullProperty =
        AvaloniaProperty.RegisterDirect<MainWindow, bool>(
            nameof(ShowSkull),
            editor => editor.ShowSkull,
            (editor, value) => editor.ShowSkull = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<MainWindow, ICommand> ToggleTabCommandProperty =
        AvaloniaProperty.RegisterDirect<MainWindow, ICommand>(
            nameof(ToggleTabCommand),
            editor => editor.ToggleTabCommand);

    private bool _showHint = true;
    private bool _showSkull = true;

    public MainWindow() {
        this.DataContext = this;
        this.ToggleTabCommand = ReactiveCommand.Create(this.ToggleTab);
        this.Game = new AvaloniaGame();
        this.Game.Project.Settings.PixelsPerUnit = 32;

        this.SkullViewModel = new SkullViewModel(this.Game);
        this.SolidViewModel = new SolidViewModel(this.Game);
        this.ResetScene();
        this.InitializeComponent();
    }

    public IAvaloniaGame Game { get; }

    public SkullViewModel SkullViewModel { get; }

    public SolidViewModel SolidViewModel { get; }

    public ICommand ToggleTabCommand { get; }

    public bool ShowHint {
        get => this._showHint;
        private set => this.SetAndRaise(ShowHintProperty, ref this._showHint, value);
    }

    public bool ShowSkull {
        get => this._showSkull;
        set => this.SetAndRaise(ShowSkullProperty, ref this._showSkull, value);
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetScene() {
        this.Game.LoadScene(this.ShowSkull ? this.SkullViewModel.Scene : this.SolidViewModel.Scene);
    }

    private void ToggleTab() {
        if (this._showHint) {
            this.ShowHint = false;
        }

        this.ShowSkull = !this.ShowSkull;
        this.ResetScene();
    }
}