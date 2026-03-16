using CartSyncBackend.Database.Interfaces;

namespace CartSyncBackend.Utils;

public static class Util
{
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
}