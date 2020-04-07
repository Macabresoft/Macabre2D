namespace Macabre2D.UI.Library.ServiceInterfaces {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;

    public interface IValueEditorService {

        Task<DependencyObject> CreateEditor(object editableObject, string name, Type declaringType);

        Task<IList<DependencyObject>> CreateEditors(object editableObject, Type declaringType, params Type[] typesToIgnore);
    }
}