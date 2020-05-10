namespace Macabre2D.UI.CosmicSynthLibrary.Controls.SongEditing {

    using Microsoft.Xna.Framework;
    using System.ComponentModel;

    public interface IClickablePianoRollComponent : INotifyPropertyChanged {
        bool IsClickable { get; }

        int Priority { get; }

        void EndClick();

        bool TryClick(Vector2 mouseWorldPosition);

        bool TryHoldClick(Vector2 mouseWorldPosition);
    }
}