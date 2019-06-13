namespace Macabre2D.UI.ViewModels.Dialogs {

    using Macabre2D.Framework.Rendering;

    public sealed class EditTileSetViewModel : OKCancelDialogViewModel {
        public Sprite[,] Sprites { get; set; }
    }
}