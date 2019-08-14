namespace Macabre2D.UI.ViewModels.Dialogs {

    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class SelectSpriteViewModel : OKCancelDialogViewModel {
        private SpriteWrapper _selectedSprite;

        public SelectSpriteViewModel(IProjectService projectService) {
            var spriteWrappers = projectService.CurrentProject.AssetFolder.GetAssetsOfType<ImageAsset>().SelectMany(x => x.Sprites).ToList();
            spriteWrappers.Insert(0, new NullSpriteWrapper());
            this.SpriteWrappers = spriteWrappers;
        }

        public SpriteWrapper SelectedSprite {
            get {
                return this._selectedSprite;
            }

            set {
                this.Set(ref this._selectedSprite, value);
            }
        }

        public IReadOnlyCollection<SpriteWrapper> SpriteWrappers { get; }
    }
}