namespace Macabresoft.Macabre2D.Editor.UI.Mappers {
    using System;
    using Macabresoft.Macabre2D.Editor.Library.Mappers;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors;

    /// <summary>
    /// Maps a <see cref="Type"/> of a <see cref="IValueEditor"/> for special scenarios.
    /// </summary>
    public class ValueEditorTypeMapper : IValueEditorTypeMapper {
        /// <inheritdoc />
        public Type EnumEditorType => typeof(EnumEditor);

        /// <inheritdoc />
        public Type FlagsEnumEditorType => typeof(FlagsEnumEditor);
        
        /// <inheritdoc />
        public Type GenericEditorType => typeof(GenericValueEditor);
    }
}