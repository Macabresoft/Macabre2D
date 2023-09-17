namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a single project in the engine.
/// </summary>
public interface IGameProject : INotifyPropertyChanged {
    /// <summary>
    /// Gets the common view height used when a camera does not override with its own value.
    /// </summary>
    float CommonViewHeight { get; }

    /// <summary>
    /// Gets the default user settings.
    /// </summary>
    UserSettings DefaultUserSettings { get; }

    /// <summary>
    /// Gets the input settings.
    /// </summary>
    InputSettings InputSettings { get; }

    /// <summary>
    /// Gets the layer settings.
    /// </summary>
    LayerSettings LayerSettings { get; }

    /// <summary>
    /// Gets the name for this project.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether or not this should pixel snap.
    /// </summary>
    bool SnapToPixels { get; }

    /// <summary>
    /// Gets the content identifier of the scene loaded on startup.
    /// </summary>
    Guid StartupSceneContentId { get; }

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
    /// Gets or sets the color that sprites will be filled in with if their content cannot be loaded.
    /// </summary>
    Color ErrorSpritesColor { get; set; }

    /// <summary>
    /// Gets or sets the color of the game background when there is no scene opened.
    /// </summary>
    Color FallbackBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the pixels per unit. This value is the number of pixels per arbitrary game units.
    /// </summary>
    ushort PixelsPerUnit { get; set; }

    /// <summary>
    /// Gets a pixel agnostic ratio. This can be used to make something appear the same size on
    /// screen regardless of the current view size.
    /// </summary>
    /// <param name="unitViewHeight">Height of the unit view.</param>
    /// <param name="pixelViewHeight">Height of the pixel view.</param>
    /// <returns>A pixel agnostic ratio.</returns>
    float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight);
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

    private float _commonViewHeight = 10f;
    private ushort _pixelsPerUnit = 32;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameProject" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="startupSceneContentId">The identifier for the scene which should run on startup.</param>
    public GameProject(string name, Guid startupSceneContentId) {
        this.Name = name;
        this.StartupSceneContentId = startupSceneContentId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameProject" /> class.
    /// </summary>
    public GameProject() : this(DefaultProjectName, Guid.Empty) {
    }

    [DataMember]
    public UserSettings DefaultUserSettings { get; } = new();

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Input)]
    public InputSettings InputSettings { get; } = new();

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Layers)]
    public LayerSettings LayerSettings { get; } = new();

    /// <inheritdoc />
    [DataMember]
    public float CommonViewHeight {
        get => this._commonViewHeight;
        set => this._commonViewHeight = Math.Max(value, 0.1f); // View height cannot be 0, that would be chaos.
    }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Fallback)]
    public Color ErrorSpritesColor { get; set; } = Color.HotPink;

    /// <inheritdoc />
    [DataMember(Name = nameof(FallbackBackgroundColor))]
    [Category(CommonCategories.Fallback)]
    public Color FallbackBackgroundColor { get; set; } = Color.Black;

    /// <inheritdoc />
    [DataMember]
    public string Name { get; set; }

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
            }
        }
    }

    /// <inheritdoc />
    [DataMember(Name = "Snap to Pixels During Render")]
    public bool SnapToPixels { get; set; }

    /// <inheritdoc />
    [DataMember(Name = "Startup Scene")]
    [AssetGuid(typeof(SceneAsset))]
    public Guid StartupSceneContentId { get; set; }

    /// <inheritdoc />
    public float UnitsPerPixel { get; private set; } = 1f / 32f;

    /// <inheritdoc />
    public float GetPixelAgnosticRatio(float unitViewHeight, int pixelViewHeight) {
        return unitViewHeight * ((float)this.PixelsPerUnit / pixelViewHeight);
    }
}