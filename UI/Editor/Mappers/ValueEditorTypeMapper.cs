namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using Macabresoft.Macabre2D.UI.Common;
    using Macabresoft.Macabre2D.UI.Common.Mappers;

    /// <summary>
    /// Maps a <see cref="Type" /> of a <see cref="IValueEditor" /> for special scenarios.
    /// </summary>
    public class ValueEditorTypeMapper : IValueEditorTypeMapper {
        /// <inheritdoc />
        public Type AssetReferenceType => typeof(AssetGuidEditor);

        /// <inheritdoc />
        public Type EnumEditorType => typeof(EnumEditor);

        /// <inheritdoc />
        public Type FlagsEnumEditorType => typeof(FlagsEnumEditor);
    }
}