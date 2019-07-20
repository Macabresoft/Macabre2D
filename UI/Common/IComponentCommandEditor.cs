namespace Macabre2D.UI.Common {

    using Macabre2D.Framework;
    using System.Collections.Generic;

    public interface IComponentCommandEditor<T> where T : BaseComponent {
        IReadOnlyCollection<ComponentCommand<T>> Commands { get; }
    }
}