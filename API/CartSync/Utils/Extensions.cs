global using UsageResponse = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<(System.Ulid, string)>>;
using CartSync.Models.Interfaces;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSync.Utils;

public static class Extensions
{
    extension(string str)
    {
        private string Pluralize()
        {
            return str.EndsWith('y') ? string.Concat(str.AsSpan(0, str.Length - 1), "ies") : str + "s";
        }
    }
    
    extension(UsageResponse usages)
    {
        public void Update<T>(IEnumerable<T> items, Func<T, Ulid> getId, Func<T, string> getName)
        {
            string keyName = typeof(T).Name.Pluralize();
            foreach (T item in items)
            {
                if (usages.TryGetValue(keyName, out List<(Ulid, string)>? usage))
                {
                    (Ulid, string) candidate = (getId.Invoke(item), getName.Invoke(item));

                    if (!usage.Contains(candidate))
                    {
                        usage.Add(candidate);
                    }
                }
                else
                {
                    usages[keyName] = [ (getId.Invoke(item), getName.Invoke(item)) ];
                }
            }
        }
    }
    
    extension(IEnumerable<ISortable> list)
    {
        public void RefreshOrder()
        {
            int index = 0;
            foreach (ISortable sortable in list.OrderBy(l => l.SortOrder))
            {
                sortable.SortOrder = index++;
            }
        }

        public void Reorder(int oldIndex, int newIndex)
        {
            if (newIndex < 0)
            {
                newIndex = 0;
            }

            ISortable[] elements = list.OrderBy(l => l.SortOrder).ToArray();
            if (newIndex > elements.Length - 1)
            {
                newIndex = elements.Length - 1;
            }
        
            if (oldIndex == newIndex)
            {
                return;
            }

            if (oldIndex < newIndex)
            {
                while (oldIndex < newIndex)
                {
                    (elements[oldIndex].SortOrder, elements[oldIndex + 1].SortOrder) = (elements[oldIndex + 1].SortOrder, elements[oldIndex].SortOrder);
                    (elements[oldIndex], elements[oldIndex + 1]) = (elements[oldIndex + 1], elements[oldIndex]);
                    oldIndex++;
                }
            }
            else
            {
                while (oldIndex > newIndex)
                {
                    (elements[oldIndex].SortOrder, elements[oldIndex - 1].SortOrder) = (elements[oldIndex - 1].SortOrder, elements[oldIndex].SortOrder);
                    (elements[oldIndex], elements[oldIndex - 1]) = (elements[oldIndex - 1], elements[oldIndex]);
                    oldIndex--;
                }
            }
        }
    }

    [UsedImplicitly]
    public class UlidValueConverter() : ValueConverter<Ulid, string>(
        v => v.ToString(),
        v => Ulid.Parse(v)
    );
}