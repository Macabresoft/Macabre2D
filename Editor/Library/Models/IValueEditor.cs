namespace Macabresoft.Macabre2D.Editor.Library.Models {
    using System.ComponentModel;

    public interface IValueEditor<T> : INotifyPropertyChanged {
        object Owner { get; set; }

        T Value { get; }

        string ValuePropertyName { get; set; }
    }
}