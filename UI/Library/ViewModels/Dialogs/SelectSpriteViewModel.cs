namespace Macabre2D.UI.Library.ViewModels.Dialogs {

    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.Models.FrameworkWrappers;
    using Macabre2D.UI.Library.ServiceInterfaces;
    using Macabre2D.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class SelectSpriteViewModel : OKCancelDialogViewModel {
        private readonly NullSpriteWrapper _nullSpriteWrapper = new NullSpriteWrapper();
        private SpriteWrapper _selectedSprite;

        public SelectSpriteViewModel(IProjectService projectService) {
            var spriteWrappers = projectService.CurrentProject.AssetFolder.GetAssetsOfType<ImageAsset>().SelectMany(x => x.Sprites).ToList();
            spriteWrappers.Insert(0, this._nullSpriteWrapper);
            this.SpriteWrappers = spriteWrappers;
            this.FilterFunc = new Func<object, string, bool>(this.CheckValueMatchesFilter);
        }

        public Func<object, string, bool> FilterFunc { get; }

        public SpriteWrapper SelectedSprite {
            get {
                return this._selectedSprite;
            }

            set {
                if (value == null) {
                    this.Set(ref this._selectedSprite, this._nullSpriteWrapper);
                }
                else if (this.SpriteWrappers.Contains(value)) {
                    this.Set(ref this._selectedSprite, value);
                }
            }
        }

        public IReadOnlyCollection<SpriteWrapper> SpriteWrappers { get; }

        private bool CheckValueMatchesFilter(object value, string filterText) {
            var result = false;
            if (value is SpriteWrapper sprite) {
                result = sprite.Name?.Contains(filterText, StringComparison.OrdinalIgnoreCase) == true;
            }

            return result;
        }
    }
}