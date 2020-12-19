namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Framework;

    public interface IValueEditorService {
        Task<IValueEditor<T>> CreateEditor<T>(object editableObject, string name, Type declaringType);

        /// <summary>
        /// Creates value editors for an object based on its properties and fields that contain a <see cref="DataMemberAttribute"/>.
        /// </summary>
        /// <param name="gameComponent">The game component for which to make editors..</param>
        /// <param name="declaringType">The declaring type of the object being edited.</param>
        /// <param name="typesToIgnore"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<IValueEditor>> CreateEditors(IGameComponent gameComponent, Type declaringType, params Type[] typesToIgnore);
    }
    
    public class ValueEditorService {
        
    }
}