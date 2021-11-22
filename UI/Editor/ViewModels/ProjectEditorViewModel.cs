namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using System.ComponentModel;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop;
    using Macabresoft.Macabre2D.UI.Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// View model for project editing.
    /// </summary>
    public class ProjectEditorViewModel : BaseViewModel {
        private const float ViewHeightRequired = 10f;
        private readonly IEditorService _editorService;
        private readonly IEditorGame _game;
        private readonly IScene _scene;
        private ICamera _camera;
        private SpriteAnimator _spriteAnimator;
        private AutoTileMap _tileMap;

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

        private IScene CreateScene() {
            var scene = new Scene {
                BackgroundColor = this.BackgroundColor
            };

            scene.AddSystem<RenderSystem>();
            scene.AddSystem<UpdateSystem>();

            this._camera = scene.AddChild<Camera>();
            this._camera.ViewHeight = ViewHeightRequired;
            this._camera.OffsetSettings.OffsetType = PixelOffsetType.BottomLeft;
            this._camera.LocalPosition = new Vector2(-0.5f);

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
            }
        }

        private void ResetScene(AutoTileSet tileSet) {
            if (tileSet.SpriteSheet is SpriteSheet spriteSheet) {
                this._tileMap.TileSetReference.Clear();
                this._tileMap.TileSetReference.PackagedAssetId = tileSet.Id;
                this._tileMap.TileSetReference.Initialize(spriteSheet);
                this._scene.Assets.ResolveAsset<SpriteSheet, Texture2D>(this._tileMap.TileSetReference);

                this._tileMap.IsEnabled = true;
            }
        }

        private void ResetScene(SpriteAnimation spriteAnimation) {
            this._spriteAnimator.Play(spriteAnimation, true);
        }

        private void ResetScene(SpriteSheet spriteSheet) {
        }
    }
}