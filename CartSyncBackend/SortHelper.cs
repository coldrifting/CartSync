namespace CartSyncBackend;

public static class SortHelper
{
    public static int[] Reorder(int arraySize, int index, int newIndex)
    {
        int[] result = Enumerable.Range(0, arraySize).ToArray();

        if (index == newIndex)
        {
            return result;
        }

        if (newIndex < index)
        {
            for (int i = index; i > newIndex; i--)
            {
                (result[i], result[i - 1]) = (result[i - 1], result[i]);
            }
        }
        else
        {
            for (int i = index; i < newIndex; i++)
            {
                (result[i], result[i + 1]) = (result[i + 1], result[i]);
            }
        }
        
        return result;
    }
}