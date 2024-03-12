namespace SystemMonitor.Logic.Utilities
{
    internal class OrderedStringArray(string[] items)
    {
        public bool AddIfNotExist(string item)
        {
            string[] newItems = new string[items.Length + 1];

            int i = 0;

            for (; i < items.Length; i++)
            {
                string currentItem = items[i];
                int comparison = currentItem.CompareTo(item);

                if (comparison == 0)
                {
                    return false;
                }

                if (comparison > 0)
                {
                    break;
                }

                newItems[i] = currentItem;
            }

            newItems[i++] = item;

            for (; i <= items.Length; i++)
            {
                newItems[i] = items[i - 1];
            }

            items = newItems;

            return true;
        }

        public string[] GetItems()
        {
            return items;
        }
    }
}