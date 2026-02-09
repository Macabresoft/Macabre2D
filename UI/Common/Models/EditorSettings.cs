namespace Macabre2D.UI.Common;

using System;
using System.Linq;
using System.Runtime.Serialization;
using Avalonia.Controls;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Settings for the editor. What a novel idea!
/// </summary>
[DataContract]
public class EditorSettings {
    /// <summary>
    /// It's the editor settings file name.
    /// </summary>
    public const string FileName = "settings.m2deditor";

    /// <summary>
    /// Instantiates a new instance of the <see cref="EditorSettings" /> class.
    /// </summary>
    public EditorSettings() {
        if (EnumHelper.TryGetAll(typeof(Layers), out var result) && result is Layers layers) {
            this.LayersToRender = layers;
        }
    }

    /// <summary>
    /// Gets or sets the animation preview frame rate.
    /// </summary>
    [DataMember]
    public byte AnimationPreviewFrameRate { get; set; } = 8;

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    [DataMember]
    public Color BackgroundColor { get; set; } = PredefinedColors.Colors.Any() ? PredefinedColors.Colors.First() : Color.CornflowerBlue;

    /// <summary>
    /// Gets or sets the position of the editor camera.
    /// </summary>
    [DataMember]
    public Vector2 CameraPosition { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the view height of the editor camera.
    /// </summary>
    [DataMember]
    public float CameraViewHeight { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the input display.
    /// </summary>
    [DataMember]
    public InputDevice InputDeviceDisplay { get; set; } = InputDevice.GamePad;

    /// <summary>
    /// Gets or sets the last gizmo opened.
    /// </summary>
    [DataMember]
    public GizmoKind LastGizmoSelected { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the last content opened.
    /// </summary>
    [DataMember]
    public Guid LastContentOpenedId { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether a prefab was opened last. If false, a prefab was open.
    /// </summary>
    [DataMember]
    public bool WasSceneOpenedLast { get; set; } = true;

    /// <summary>
    /// Gets or sets the last tab selected.
    /// </summary>
    [DataMember]
    public EditorTabs LastTabSelected { get; set; } = EditorTabs.Scene;

    /// <summary>
    /// Gets or sets the layers to render.
    /// </summary>
    [DataMember]
    public Layers LayersToRender { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not content should be rebuilt on next load.
    /// </summary>
    [DataMember]
    public bool ShouldRebuildContent { get; set; }

    /// <summary>
    /// Gets or sets the window state.
    /// </summary>
    [DataMember]
    public WindowState WindowState { get; set; } = WindowState.Maximized;
}