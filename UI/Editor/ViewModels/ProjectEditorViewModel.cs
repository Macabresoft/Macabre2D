namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using System.ComponentModel;
    using Avalonia;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
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
        private ICamera _camera;
        private EditorGrid _grid;
        private Rect _overallSceneArea;
        private SpriteAnimator _spriteAnimator;
        private AutoTileMap _tileMap;
        private Rect _viewableSceneArea;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorViewModel" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="game">The game.</param>
        /// <param name="projectService">The project service.</param>
        public ProjectEditorViewModel(
            IEditorService editorService,
            IEditorGame game,
            IProjectService projectService) : base() {
            this._editorService = editorService ?? throw new ArgumentNullException(nameof(editorService));
            this._game = game ?? throw new ArgumentNullException(nameof(game));
            this.ProjectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            this.ProjectService.PropertyChanged += this.ProjectService_PropertyChanged;
            this._scene = this.CreateScene();

            if (this._editorService.SelectedTab == EditorTabs.Project) {
                this._game.LoadScene(this._scene);
            }

            this._editorService.PropertyChanged += this.EditorService_PropertyChanged;
        }

        /// <summary>
        /// Gets the background color.
        /// </summary>
        public Color BackgroundColor => DefinedColors.MacabresoftPurple;

        /// <summary>
        /// Gets the selection service.
        /// </summary>
        public IProjectService ProjectService { get; }

        /// <summary>
        /// Gets or sets the overall area of the scene.
        /// </summary>
        public Rect OverallSceneArea {
            get => this._overallSceneArea;
            set => this.ResetSize(value, this.ViewableSceneArea);
        }

        /// <summary>
        /// Gets or sets the viewable area of the scene.
        /// </summary>
        public Rect ViewableSceneArea {
            get => this._viewableSceneArea;
            set => this.ResetSize(this.OverallSceneArea, value);
        }

        private IScene CreateScene() {
            var scene = new Scene {
                BackgroundColor = this.BackgroundColor
            };

            scene.AddSystem<RenderSystem>();
            scene.AddSystem<UpdateSystem>();

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

            this._spriteAnimator = scene.AddChild<SpriteAnimator>();
            this._spriteAnimator.FrameRate = 8;
            this._spriteAnimator.IsEnabled = false;
            return scene;
        }

        private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IEditorService.SelectedTab) && this._editorService.SelectedTab == EditorTabs.Project) {
                this._game.LoadScene(this._scene);
            }
        }

        private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IProjectService.Selected)) {
                this._tileMap.IsEnabled = false;
                this._spriteAnimator.IsEnabled = false;
                this._grid.IsEnabled = false;
                switch (this.ProjectService.Selected) {
                    case AutoTileSet autoTileSet:
                        this.ResetScene(autoTileSet);
                        break;
                    case SpriteAnimation spriteAnimation:
                        this.ResetScene(spriteAnimation);
                        break;
                    case SpriteSheet spriteSheet:
                        this.ResetScene(spriteSheet);
                        break;
                }

                this.ResetSize(this.OverallSceneArea, this.ViewableSceneArea);
            }
        }

        private void ResetScene(AutoTileSet tileSet) {
            if (tileSet.SpriteSheet != null) {
                this._tileMap.TileSetReference.Reset(tileSet);
                this._scene.Assets.ResolveAsset<SpriteSheet, Texture2D>(this._tileMap.TileSetReference);
                this._tileMap.IsEnabled = true;
                this._grid.IsEnabled = true;
            }
        }

        private void ResetScene(SpriteAnimation spriteAnimation) {
            if (spriteAnimation.SpriteSheet != null) {
                this._spriteAnimator.AnimationReference.Reset(spriteAnimation);
                this._scene.Assets.ResolveAsset<SpriteSheet, Texture2D>(this._spriteAnimator.AnimationReference);
                this._spriteAnimator.Play(spriteAnimation, true);
                this._spriteAnimator.IsEnabled = true;
            }
        }

        private void ResetScene(SpriteSheet spriteSheet) {
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
                    this._camera.OffsetSettings.Offset = -1f * new Vector2(overallWidth - viewableWidth, overallHeight - viewableHeight);
                }
                else {
                    this._camera.ViewHeight = ViewHeightRequired;
                }
            }
        }

        private float GetRequiredViewHeight() {
            return this._spriteAnimator.IsEnabled ? this._spriteAnimator.BoundingArea.Height + 1f : ViewHeightRequired;
        }
    }
}