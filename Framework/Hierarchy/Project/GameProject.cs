namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Common.Attributes;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a single project in the engine.
/// </summary>
public interface IGameProject : INotifyPropertyChanged {
    /// <summary>
    /// Gets any additional configuration specific to a project.
    /// </summary>
    ProjectConfiguration AdditionalConfiguration { get; }

    /// <summary>
    /// Gets all layers available in this project as a single enum.
    /// </summary>
    Layers AllLayers { get; }

    /// <summary>
    /// Gets a value indicating whether input is allowed regardless of device. If this is set to <c>true</c>, the
    /// desired input device in <see cref="InputSettings" /> is purely for display and all inputs will be available.
    /// </summary>
    bool AllowInputRegardlessOfDevice { get; }

    /// <summary>
    /// Gets the company name.
    /// </summary>
    string CompanyName { get; }

    /// <summary>
    /// Gets the default user settings.
    /// </summary>
    UserSettings DefaultUserSettings { get; }

    /// <summary>
    /// Gets the fallback values for colors, fonts, icon sets, and more.
    /// </summary>
    ProjectFallbacks Fallbacks { get; }

    /// <summary>
    /// Gets a collection of fonts defined by <see cref="FontCategory" /> and <see cref="ResourceCulture" />.
    /// </summary>
    ProjectFonts Fonts { get; }

    /// <summary>
    /// Gets the internal render resolution. This is the resolution the game is rendered at before being scaled up to the player's window.
    /// </summary>
    Point InternalRenderResolution { get; }

    /// <summary>
    /// Gets the ratio of the internal render resolution. This is width / height. So, if your render resolution is 800x600, the value of this will be equal to 800/600 or 4/3.
    /// </summary>
    float InternalRenderResolutionRatio { get; }

    /// <summary>
    /// Gets the name for this project.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the scene identifier of the scene to render as an overlay.
    /// </summary>
    Guid PersistentOverlaySceneId { get; }

    /// <summary>
    /// Gets or sets the pixels per unit. This value is the number of pixels per arbitrary game units.
    /// </summary>
    ushort PixelsPerUnit { get; set; }

    /// <summary>
    /// Gets the screen shaders for this project.
    /// </summary>
    RenderStepCollection RenderSteps { get; }
    
    /// <summary>
    /// Gets the physics materials.
    /// </summary>
    PhysicsMaterialCollection PhysicsMaterials { get; }

    /// <summary>
    /// Gets the identifier of the scene to load on a debug startup.
    /// </summary>
    public Guid StartupDebugSceneId { get; }

    /// <summary>
    /// Gets the identifier of the scene to load on startup.
    /// </summary>
    Guid StartupSceneId { get; }

    /// <summary>
    /// Gets the inverse of <see cref="PixelsPerUnit" />.
    /// </summary>
    /// <remarks>
    /// This will be calculated when <see cref="PixelsPerUnit" /> is set.
    /// Multiplication is a quicker operation than division, so if you find yourself dividing by
    /// <see cref="PixelsPerUnit" /> regularly, consider multiplying by this instead as it will
    /// produce the same value, but quicker.
    /// </remarks>
    float UnitsPerPixel { get; }

    /// <summary>
    /// Gets the view height based on render resolution and units per pixel.
    /// </summary>
    float ViewHeight { get; }

    /// <summary>
    /// Gets a pixel agnostic ratio. This can be used to make something appear the same size on
    /// screen regardless of the current view size.
    /// </summary>
    /// <param name="unitViewHeight">Height of the unit view.</param>
    /// <param name="pixelViewHeight">Height of the pixel view.</param>
    /// <returns>A pixel agnostic ratio.</returns>
    float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight);

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="assets">The asset manager.</param>
    /// <param name="game">The game</param>
    void Initialize(IAssetManager assets, IGame game);
}

/// <summary>
/// Defines a single project within the engine.
/// </summary>
[DataContract]
[Category(CommonCategories.Miscellaneous)]
public class GameProject : PropertyChangedNotifier, IGameProject {
    /// <summary>
    /// The default project name.
    /// </summary>
    public const string DefaultProjectName = "Project";

    /// <summary>
    /// The project file extension.
    /// </summary>
    public const string ProjectFileExtension = ".m2dproj";

    /// <summary>
    /// The project file name.
    /// </summary>
    public const string ProjectFileName = DefaultProjectName + ProjectFileExtension;

    /// <summary>
    /// Gets an empty game project.
    /// </summary>
    public static readonly IGameProject Empty = new GameProject(string.Empty, Guid.Empty);

    private Point _internalRenderResolution = new(800, 600);
    private ushort _pixelsPerUnit = 32;
    private float _viewHeight = 10f;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameProject" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="startupSceneId">The identifier for the scene which should run on startup.</param>
    public GameProject(string name, Guid startupSceneId) {
        this.Name = name;
        this.StartupSceneId = startupSceneId;
        this.AllLayers = Enum.GetValues<Layers>().Aggregate((current, layer) => current | layer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameProject" /> class.
    /// </summary>
    public GameProject() : this(DefaultProjectName, Guid.Empty) {
    }

    /// <inheritdoc />
    [DataMember]
    public ProjectConfiguration AdditionalConfiguration { get; } = new();

    /// <inheritdoc />
    public Layers AllLayers { get; }

    /// <inheritdoc />
    [DataMember]
    public bool AllowInputRegardlessOfDevice { get; set; }

    /// <inheritdoc />
    [DataMember]
    public string CompanyName { get; set; } = string.Empty;

    /// <inheritdoc />
    [DataMember]
    public UserSettings DefaultUserSettings { get; } = new();

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Fallback)]
    public ProjectFallbacks Fallbacks { get; } = new();

    /// <inheritdoc />
    [DataMember]
    [Category("Font Definitions")]
    public ProjectFonts Fonts { get; } = new();

    /// <inheritdoc />
    [DataMember]
    public Point InternalRenderResolution {
        get => this._internalRenderResolution;
        set {
            this._internalRenderResolution = new Point(Math.Max(1, value.X), Math.Max(1, value.Y));
            this.InternalRenderResolutionRatio = this._internalRenderResolution.X / (float)this._internalRenderResolution.Y;
            this.ResetViewSize();
        }
    }

    public float InternalRenderResolutionRatio { get; private set; }

    /// <inheritdoc />
    [DataMember]
    public string Name { get; set; }

    /// <inheritdoc />
    [DataMember(Name = "Persistent Overlay")]
    [SceneGuid]
    public Guid PersistentOverlaySceneId { get; set; }

    /// <inheritdoc />
    [DataMember]
    public ushort PixelsPerUnit {
        get => this._pixelsPerUnit;
        set {
            if (value < 1) {
                throw new ArgumentOutOfRangeException($"{nameof(this.PixelsPerUnit)} must be greater than 0!");
            }

            if (value != this._pixelsPerUnit) {
                this._pixelsPerUnit = value;
                this.UnitsPerPixel = 1f / this._pixelsPerUnit;
                this.ResetViewSize();
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public RenderStepCollection RenderSteps { get; } = [];

    /// <inheritdoc />
    [DataMember]
    public PhysicsMaterialCollection PhysicsMaterials { get; } = [];

    /// <inheritdoc />
    [DataMember(Name = "Debug Scene")]
    [SceneGuid]
    public Guid StartupDebugSceneId { get; set; }

    /// <inheritdoc />
    [DataMember(Name = "Startup Scene")]
    [SceneGuid]
    public Guid StartupSceneId { get; set; }

    /// <inheritdoc />
    public float UnitsPerPixel { get; private set; } = 1f / 32f;

    /// <inheritdoc />
    public float ViewHeight {
        get => this._viewHeight;
        private set => this._viewHeight = Math.Max(value, 0.1f); // View height cannot be 0, that would be chaos.
    }

    /// <inheritdoc />
    public float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight) => unitViewHeight * ((float)this.PixelsPerUnit / pixelViewHeight);

    /// <inheritdoc />
    public void Initialize(IAssetManager assets, IGame game) {
        this.Fallbacks.GamePadNReference.Initialize(assets, game);
        this.Fallbacks.GamePadSReference.Initialize(assets, game);
        this.Fallbacks.GamePadXReference.Initialize(assets, game);
        this.Fallbacks.KeyboardReference.Initialize(assets, game);
        this.Fallbacks.MouseReference.Initialize(assets, game);
        this.Fallbacks.MouseCursorReference.Initialize(assets, game);
        this.Fallbacks.Font.Initialize(assets, game);
    }

    private void ResetViewSize() {
        this.ViewHeight = this._internalRenderResolution.Y * this.UnitsPerPixel;
    }
}