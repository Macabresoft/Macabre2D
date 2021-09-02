namespace Macabresoft.Macabre2D.UI.ProjectEditor {

    using Avalonia.Controls;
    using Avalonia.Controls.Templates;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;
    using System;

    public class ViewLocator : IDataTemplate {
        public bool SupportsRecycling => false;

        public IControl Build(object data) {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null) {
                return (Control)Activator.CreateInstance(type);
            }
            else {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data) {
            return data is BaseViewModel;
        }
    }
}