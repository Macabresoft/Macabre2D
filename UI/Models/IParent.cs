namespace Macabre2D.UI.Models {

    using System.Collections.Generic;

    public interface IParent<TChildren> {
        IReadOnlyCollection<TChildren> Children { get; }

        bool AddChild(TChildren child);

        bool RemoveChild(TChildren child);
    }
}