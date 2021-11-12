namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.Runtime.Serialization;

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
        /// Gets or sets the last gizmo opened.
        /// </summary>
        [DataMember]
        public GizmoKind LastGizmoSelected { get; set; }

        /// <summary>
        /// Gets or sets the last scene opened.
        /// </summary>
        [DataMember]
        public Guid LastSceneOpened { get; set; }

        /// <summary>
        /// Gets or sets the last tab selected.
        /// </summary>
        [DataMember]
        public EditorTabs LastTabSelected { get; set; } = EditorTabs.Scene;
    }
}