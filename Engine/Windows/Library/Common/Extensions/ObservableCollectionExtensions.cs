namespace Macabre2D.Engine.Windows.Library.Common.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public static class ObservableCollectionExtensions {

        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison) {
            var sortedList = new List<T>(collection);
            sortedList.Sort(comparison);

            for (var i = 0; i < sortedList.Count; i++) {
                collection.Move(collection.IndexOf(sortedList[i]), i);
            }
        }
    }
}