namespace Macabresoft.Macabre2D.UI.ProjectEditor.Mappers {
    using System;
    using Macabresoft.Macabre2D.UI.Common.Mappers;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors;

    /// <summary>
    /// Maps a <see cref="Type"/> of a <see cref="IValueEditor"/> for special scenarios.
    /// </summary>
    public class ValueEditorTypeMapper : IValueEditorTypeMapper {
        /// <inheritdoc />
        public Type EnumEditorType => typeof(EnumEditor);

        /// <inheritdoc />
        public Type FlagsEnumEditorType => typeof(FlagsEnumEditor);
    }
}