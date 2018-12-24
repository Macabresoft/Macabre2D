namespace Macabre2D.UI.ServiceInterfaces {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;

    public interface IValueEditorService {

        Task<DependencyObject> CreateEditor(object editableObject, string name, Type declaringTypeToIgnore);

        Task<IEnumerable<DependencyObject>> CreateEditors(object editableObject, Type declaringTypeToIgnore);
    }
}