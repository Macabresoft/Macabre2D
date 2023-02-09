namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Microsoft.Xna.Framework;
using ReactiveUI;
using Point = Microsoft.Xna.Framework.Point;

/// <summary>
/// View model for project editing.
/// </summary>
public class ProjectEditorViewModel : BaseViewModel {
    private const float ViewHeightRequired = 10f;
    private static readonly Vector2 CameraAdjustment = new(-0.5f);
    private readonly IEditorService _editorService;
    private readonly IEditorGame _game;
    private readonly IScene _scene;
    private readonly IEditorSettingsService _settingsService;
    private ICamera _camera;
    private EditorGrid _grid;
    private Rect _overallSceneArea;
    private LoopingSpriteAnimator _spriteAnimator;
    private AutoTileMap _tileMap;
    private Rect _viewableSceneArea;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectEditorViewModel" /> class.
    /// </summary>
    /// <param name="assetSelectionService">The asset selection service.</param>
    /// <param name="editorService">The editor service.</param>
    /// <param name="game">The game.</param>
    /// <param name="settingsService">The settings service.</param>
    public ProjectEditorViewModel(
        IAssetSelectionService assetSelectionService,
        IEditorService editorService,
        IEditorGame game,
        IEditorSettingsService settingsService) : base() {
        this.AssetSelectionService = assetSelectionService;
        this._editorService = editorService;
        this._game = game;
        this._settingsService = settingsService;

        this.AssetSelectionService.PropertyChanged += this.AssetSelectionService_PropertyChanged;
        this._scene = this.CreateScene();

        this.PlayCommand = ReactiveCommand.Create(this._spriteAnimator.Play, this._spriteAnimator.WhenAny(x => x.IsPlaying, x => !x.Value));
        this.PauseCommand = ReactiveCommand.Create(this._spriteAnimator.Pause, this._spriteAnimator.WhenAnyValue(x => x.IsPlaying));
        this.StopCommand = ReactiveCommand.Create(this._spriteAnimator.Stop, this._spriteAnimator.WhenAnyValue(x => x.IsEnabled));

        if (this._editorService.SelectedTab == EditorTabs.Project) {
            this._game.LoadScene(this._scene);
        }

        this._editorService.PropertyChanged += this.EditorService_PropertyChanged;
    }

    /// <summary>
    /// Gets or sets the animation preview framerate.
    /// </summary>
    public byte AnimationPreviewFramerate {
        get => this._settingsService.Settings.AnimationPreviewFramerate;
        set {
            this._spriteAnimator.FrameRate = value;
            this._settingsService.Settings.AnimationPreviewFramerate = this._spriteAnimator.FrameRate;
            this.RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets the asset selection service.
    /// </summary>
    public IAssetSelectionService AssetSelectionService { get; }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public Color BackgroundColor {
        get => this._settingsService.Settings.BackgroundColor;
        set {
            if (value != this.BackgroundColor) {
                this._settingsService.Settings.BackgroundColor = value;

                if (!Scene.IsNullOrEmpty(this._scene)) {
                    this._scene.BackgroundColor = this.BackgroundColor;
                }
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not an animation is showing.
    /// </summary>
    public bool IsShowingAnimation => this.AssetSelectionService.Selected is SpriteAnimation;

    /// <summary>
    /// Gets or sets the overall area of the scene.
    /// </summary>
    public Rect OverallSceneArea {
        get => this._overallSceneArea;
        set => this.ResetSize(value, this.ViewableSceneArea);
    }

    /// <summary>
    /// Gets the pause command.
    /// </summary>
    public ICommand PauseCommand { get; }

    /// <summary>
    /// Gets the play command.
    /// </summary>
    public ICommand PlayCommand { get; }

    /// <summary>
    /// Gets the stop command.
    /// </summary>
    public ICommand StopCommand { get; }

    /// <summary>
    /// Gets or sets the viewable area of the scene.
    /// </summary>
    public Rect ViewableSceneArea {
        get => this._viewableSceneArea;
        set => this.ResetSize(this.OverallSceneArea, value);
    }

    private void AssetSelectionService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IAssetSelectionService.Selected)) {
            this._tileMap.IsEnabled = false;
            this._spriteAnimator.IsEnabled = false;
            this._grid.IsEnabled = false;
            switch (this.AssetSelectionService.Selected) {
                case AutoTileSet autoTileSet:
                    this.ResetScene(autoTileSet);
                    break;
                case SpriteAnimation spriteAnimation:
                    this.ResetScene(spriteAnimation);
                    break;
                case SpriteSheetAsset spriteSheet:
                    this.ResetScene(spriteSheet);
                    break;
            }

            this.ResetSize(this.OverallSceneArea, this.ViewableSceneArea);
            this.RaisePropertyChanged(nameof(this.IsShowingAnimation));
        }
    }

    private IScene CreateScene() {
        var scene = new Scene {
            BackgroundColor = this.BackgroundColor
        };

        scene.AddLoop<RenderLoop>();
        scene.AddLoop<UpdateLoop>();

        this._camera = scene.AddChild<Camera>();
        this._camera.LocalPosition = CameraAdjustment;
        this.ResetSize(this.OverallSceneArea, this.ViewableSceneArea);

        this._grid = new EditorGrid(this._editorService, null);
        this._grid.IsEnabled = false;
        this._camera.AddChild(this._grid);

        this._tileMap = scene.AddChild<AutoTileMap>();
        this._tileMap.IsEnabled = false;
        this._tileMap.AddTile(new Point(2, 2));
        this._tileMap.AddTile(new Point(3, 2));
        this._tileMap.AddTile(new Point(4, 2));

        this._tileMap.AddTile(new Point(6, 2));

        this._tileMap.AddTile(new Point(6, 4));
        this._tileMap.AddTile(new Point(6, 5));
        this._tileMap.AddTile(new Point(6, 6));

        this._tileMap.AddTile(new Point(2, 4));
        this._tileMap.AddTile(new Point(3, 4));
        this._tileMap.AddTile(new Point(4, 4));
        this._tileMap.AddTile(new Point(2, 5));
        this._tileMap.AddTile(new Point(3, 5));
        this._tileMap.AddTile(new Point(4, 5));
        this._tileMap.AddTile(new Point(2, 6));
        this._tileMap.AddTile(new Point(3, 6));
        this._tileMap.AddTile(new Point(4, 6));

        this._tileMap.AddTile(new Point(0, 0));
        this._tileMap.AddTile(new Point(1, 0));
        this._tileMap.AddTile(new Point(2, 0));
        this._tileMap.AddTile(new Point(3, 0));
        this._tileMap.AddTile(new Point(4, 0));
        this._tileMap.AddTile(new Point(5, 0));
        this._tileMap.AddTile(new Point(6, 0));
        this._tileMap.AddTile(new Point(7, 0));
        this._tileMap.AddTile(new Point(8, 0));

        this._tileMap.AddTile(new Point(0, 1));
        this._tileMap.AddTile(new Point(0, 2));
        this._tileMap.AddTile(new Point(0, 3));
        this._tileMap.AddTile(new Point(0, 4));
        this._tileMap.AddTile(new Point(0, 5));
        this._tileMap.AddTile(new Point(0, 6));
        this._tileMap.AddTile(new Point(0, 7));
        this._tileMap.AddTile(new Point(0, 8));

        this._tileMap.AddTile(new Point(1, 8));
        this._tileMap.AddTile(new Point(2, 8));
        this._tileMap.AddTile(new Point(3, 8));
        this._tileMap.AddTile(new Point(4, 8));
        this._tileMap.AddTile(new Point(5, 8));
        this._tileMap.AddTile(new Point(6, 8));
        this._tileMap.AddTile(new Point(7, 8));
        this._tileMap.AddTile(new Point(8, 8));

        this._tileMap.AddTile(new Point(8, 1));
        this._tileMap.AddTile(new Point(8, 2));
        this._tileMap.AddTile(new Point(8, 3));
        this._tileMap.AddTile(new Point(8, 4));
        this._tileMap.AddTile(new Point(8, 5));
        this._tileMap.AddTile(new Point(8, 6));
        this._tileMap.AddTile(new Point(8, 7));
        this._tileMap.AddTile(new Point(8, 8));

        this._spriteAnimator = scene.AddChild<LoopingSpriteAnimator>();
        this._spriteAnimator.IsEnabled = false;

        // This applies the framerate to the sprite animator and also insures the frame rate is valid.
        this.AnimationPreviewFramerate = this._settingsService.Settings.AnimationPreviewFramerate;

        return scene;
    }

    private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEditorService.SelectedTab) && this._editorService.SelectedTab == EditorTabs.Project) {
            this._game.LoadScene(this._scene);
        }
    }

    private float GetRequiredViewHeight() {
        return this._spriteAnimator.IsEnabled ? this._spriteAnimator.BoundingArea.Height + 1f : ViewHeightRequired;
    }

    private void ResetScene(AutoTileSet tileSet) {
        if (tileSet.SpriteSheet != null) {
            this._tileMap.TileSetReference.Reset(tileSet);
            this._tileMap.TileSetReference.Initialize(this._scene.Assets);
            this._tileMap.IsEnabled = true;
            this._grid.IsEnabled = true;
        }
    }

    private void ResetScene(SpriteAnimation spriteAnimation) {
        if (spriteAnimation.SpriteSheet != null) {
            this._spriteAnimator.AnimationReference.Reset(spriteAnimation);
            this._spriteAnimator.AnimationReference.Initialize(this._scene.Assets);
            this._spriteAnimator.Play();
            this._spriteAnimator.IsEnabled = true;
        }
    }

    private void ResetScene(SpriteSheetAsset spriteSheet) {
    }

    private void ResetSize(Rect overallSceneArea, Rect viewableSceneArea) {
        this._overallSceneArea = overallSceneArea;
        this._viewableSceneArea = viewableSceneArea;

        if (this._camera != null) {
            var overallHeight = (float)overallSceneArea.Height;
            var overallWidth = (float)overallSceneArea.Width;
            var viewableHeight = (float)Math.Min(overallHeight, viewableSceneArea.Height);
            var viewableWidth = (float)Math.Min(overallWidth, viewableSceneArea.Width);

            if (viewableHeight > 0f && viewableWidth > 0f && overallHeight > 0f && overallWidth > 0f) {
                var heightRatio = overallHeight / viewableHeight;
                this._camera.ViewHeight = heightRatio * this.GetRequiredViewHeight();
                this._camera.OffsetOptions.Offset = -1f * new Vector2(overallWidth - viewableWidth, overallHeight - viewableHeight);
            }
            else {
                this._camera.ViewHeight = ViewHeightRequired;
            }
        }
    }
}