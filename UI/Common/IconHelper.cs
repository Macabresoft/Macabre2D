namespace Macabre2D.UI.Common {

    using System.Windows;
    using System.Windows.Shapes;

    public static class IconHelper {

        public static Path GetFileAudioIcon() {
            return Application.Current.TryFindResource("FileAudioIcon") as Path;
        }

        public static Path GetFileDocumentIcon() {
            return Application.Current.TryFindResource("FileDocumentIcon") as Path;
        }

        public static Path GetFileIcon() {
            return Application.Current.TryFindResource("FileIcon") as Path;
        }

        public static Path GetFileImageIcon() {
            return Application.Current.TryFindResource("FileImageIcon") as Path;
        }

        public static Path GetFileSceneIcon() {
            return Application.Current.TryFindResource("FileSceneIcon") as Path;
        }

        public static Path GetFileSpriteAnimationIcon() {
            return Application.Current.TryFindResource("FileSpriteAnimationIcon") as Path;
        }

        public static Path GetFolderIcon() {
            return Application.Current.TryFindResource("FolderIcon") as Path;
        }

        public static Path GetFontIcon() {
            return Application.Current.TryFindResource("FontIcon") as Path;
        }

        public static Path GetSpriteIcon() {
            return Application.Current.TryFindResource("SpriteIcon") as Path;
        }
    }
}