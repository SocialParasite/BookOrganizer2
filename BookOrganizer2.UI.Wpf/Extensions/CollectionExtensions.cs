using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookOrganizer2.UI.Wpf.Extensions
{
    public static class CollectionExtensions
    {
        public static List<T> FromListToList<T>(this IEnumerable<T> enumerableList)
        {
            return enumerableList != null ? new List<T>(enumerableList) : null;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            return enumerableList != null ? new ObservableCollection<T>(enumerableList) : null;
        }

        public static Dictionary<T, bool> ToDictionary<T>(this IEnumerable<T> enumerableList)
        {
            return enumerableList?.ToDictionary(item => item, item => false);
        }
    }
}
