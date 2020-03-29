namespace Macabre2D.Examples.AudioTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;

    public interface IClickablePianoComponent : IIdentifiableComponent {

        event EventHandler ClickabilityChanged;

        bool IsClickable { get; }

        int Priority { get; }

        void EndClick();

        bool TryClick(Vector2 mouseWorldPosition, Song currentSong, Instrument currentInstrument);

        bool TryHoldClick(Vector2 mouseWorldPosition);
    }
}