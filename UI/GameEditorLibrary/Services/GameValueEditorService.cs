namespace Macabre2D.UI.GameEditorLibrary.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Services;
    using Macabre2D.UI.GameEditorLibrary.Controls.ValueEditors;
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    public sealed class GameValueEditorService : ValueEditorService {

        public GameValueEditorService(IAssemblyService assemblyService) : base(assemblyService) {
        }

        protected override async Task<DependencyObject> GetSpecializedEditor(object originalObject, object value, Type memberType, string propertyPath, string memberName, Type declaringType) {
            DependencyObject result = null;

            if (typeof(BaseModule).IsAssignableFrom(memberType)) {
                var moduleEditor = new ModuleEditor();
                await moduleEditor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = moduleEditor;
            }
            else if (typeof(BaseComponent).IsAssignableFrom(memberType)) {
                var componentEditor = new ComponentEditor();
                await componentEditor.Initialize(value, memberType, originalObject, propertyPath, memberName);
                result = componentEditor;
            }
            else {
                result = await base.GetSpecializedEditor(originalObject, value, memberType, propertyPath, memberName, declaringType);
            }

            return result;
        }
    }
}