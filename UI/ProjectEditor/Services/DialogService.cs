namespace Macabresoft.Macabre2D.UI.ProjectEditor.Services {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Macabresoft.Macabre2D.UI.ProjectEditor.Views;
    using Unity.Resolution;

    /// <summary>
    /// A dialog service.
    /// </summary>
    public class DialogService : IDialogService {
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService" /> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        public DialogService(MainWindow mainWindow) {
            this._mainWindow = mainWindow;
        }

        /// <inheritdoc />
        public async Task<IContentNode> OpenAssetSelectionDialog(Type baseAssetType, bool allowDirectorySelection) {
            IContentNode selectedNode = null;
            var window = Resolver.Resolve<AssetSelectionDialog>(new ParameterOverride(typeof(Type), baseAssetType), new ParameterOverride(typeof(bool), allowDirectorySelection));
            var result = await window.ShowDialog<bool>(this._mainWindow);

            if (result && window.ViewModel != null) {
                selectedNode = window.ViewModel.SelectedContentNode?.Node;
            }

            return selectedNode;
        }

        /// <inheritdoc />
        public async Task<Type> OpenTypeSelectionDialog(Type baseType, params Type[] typesToIgnore) {
            Type selectedType = null;
            var window = Resolver.Resolve<TypeSelectionDialog>(new ParameterOverride(typeof(Type), baseType), new ParameterOverride(typeof(IEnumerable<Type>), typesToIgnore));
            var result = await window.ShowDialog<bool>(this._mainWindow);

            if (result && window.ViewModel != null) {
                selectedType = window.ViewModel.SelectedType;
            }

            return selectedType;
        }
    }
}