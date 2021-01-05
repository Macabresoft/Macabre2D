namespace Macabresoft.Macabre2D.Editor.Library.Mappers {
    using System;
    using Macabresoft.Macabre2D.Editor.Library.Models;

    /// <summary>
    /// Interface for something which maps a <see cref="Type"/> of a <see cref="IValueEditor"/> for special scenarios.
    /// </summary>
    public interface IValueEditorTypeMapper {
        
        /// <summary>
        /// The type for an editor of enums.
        /// </summary>
        Type EnumEditorType { get; }
        
        /// <summary>
        /// The type of an editor for enums which have the <see cref="FlagsAttribute"/>.
        /// </summary>
        Type FlagsEnumEditorType { get; }
        
        /// <summary>
        /// The type of an editor that generically populates itself with more specific editors.
        /// </summary>
        Type GenericEditorType { get; }
    }
}