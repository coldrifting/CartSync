using System.Security.Claims;
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

    extension(ClaimsPrincipal user)
    {
        public string? Username => 
            user.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
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