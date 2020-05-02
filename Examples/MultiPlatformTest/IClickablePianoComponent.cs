namespace Macabre2D.Examples.MultiPlatformTest {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;
    using System.ComponentModel;

    public interface IClickablePianoComponent : IIdentifiableComponent, INotifyPropertyChanged {

        event EventHandler ClickabilityChanged;

        bool IsClickable { get; }

        int Priority { get; }

        void EndClick();

        bool TryClick(Vector2 mouseWorldPosition, Song currentSong, Instrument currentInstrument);

        bool TryHoldClick(Vector2 mouseWorldPosition);
    }
}