namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.ComponentModel;
    using System.IO;
    using Avalonia;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Point = Microsoft.Xna.Framework.Point;

    /// <summary>
    /// An extension of <see cref="IAvaloniaGame" /> for previewing assets.
    /// </summary>
    public interface IProjectEditorGame : IAvaloniaGame {
        /// <summary>
        /// Gets or sets the overall bounds of the scene.
        /// </summary>
        Size OverallBounds { get; set; }

        /// <summary>
        /// Gets or sets the viewable bounds.
        /// </summary>
        Size ViewableBounds { get; set; }
    }

    /// <summary>
    /// An extension of <see cref="AvaloniaGame" /> for previewing assets.
    /// </summary>
    public class ProjectEditorGame : AvaloniaGame, IProjectEditorGame {
        private readonly IProjectService _projectService;
        private Size _overallBounds;
        private Size _viewableBounds;
        private bool _isInitialized;
        private ICamera _camera;
        private AutoTileMap _tileMap;
        private SpriteAnimator _spriteAnimator;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectEditorGame" /> class.
        /// </summary>
        /// <param name="assetManager">The asset manager.</param>
        /// <param name="pathService">The path service.</param>
        /// <param name="projectService">The project service.</param>
        public ProjectEditorGame(
            IAssetManager assetManager,
            IPathService pathService,
            IProjectService projectService) : base(assetManager) {
            this._projectService = projectService;
            this.Content.RootDirectory = Path.GetRelativePath(pathService.EditorBinDirectoryPath, pathService.EditorContentDirectoryPath);
        }

        /// <inheritdoc />
        public Size OverallBounds {
            get => this._overallBounds;
            set {
                this._overallBounds = value;
                this.SquashViewableBounds(this.ViewableBounds);
                this.RaisePropertyChanged();
            }
        }

        /// <inheritdoc />
        public Size ViewableBounds {
            get => this._viewableBounds;
            set => this.SquashViewableBounds(value);
        }

        /// <inheritdoc />
        protected override void Initialize() {
            if (!this._isInitialized) {
                try {
                    this.LoadScene(this.CreateScene());
                    base.Initialize();
                    this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
                }
                finally {
                    this._isInitialized = true;
                }
            }        
        }

        private IScene CreateScene() {
            var scene = new Scene{
                BackgroundColor = DefinedColors.MacabresoftPurple
            };
            
            scene.AddSystem<RenderSystem>();
            scene.AddSystem<UpdateSystem>();
            this._camera = scene.AddChild<Camera>();
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
        
        private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IProjectService.Selected)) {
                this._tileMap.IsEnabled = false;
                this._spriteAnimator.IsEnabled = false;
                switch (this._projectService.Selected) {
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
                this.Scene.Assets.ResolveAsset<SpriteSheet, Texture2D>(this._tileMap.TileSetReference);

                this._tileMap.IsEnabled = true;
            }
        }

        private void ResetScene(SpriteAnimation spriteAnimation) {
            this._spriteAnimator.Play(spriteAnimation, true);
        }

        private void ResetScene(SpriteSheet spriteSheet) {
        }

        private void SquashViewableBounds(Size requestedViewableSize) {
            this._viewableBounds = new Size(
                Math.Min(requestedViewableSize.Width, this.OverallBounds.Width),
                Math.Min(requestedViewableSize.Height, this.OverallBounds.Height));
            this.RaisePropertyChanged(nameof(this.ViewableBounds));
        }
    }
}