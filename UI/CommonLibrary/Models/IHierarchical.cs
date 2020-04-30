namespace Macabre2D.UI.CommonLibrary.Models {

    using System.ComponentModel;

    public interface IHierarchical<TChildren, TParent> : INotifyPropertyChanged, IParent<TChildren> {
        TParent Parent { get; set; }
    }
}