using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lct2023.Commons.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection) 
            => collection == null ? new ObservableCollection<T>() : new ObservableCollection<T>(collection);
    }
}