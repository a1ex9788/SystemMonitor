namespace SystemMonitor.Logic.Utilities
{
    internal class OrderedStringList(string[] items)
    {
        public string[] Items { get; private set; } = items;

        public bool AddIfNotExist(string item)
        {
            string[] newItems = new string[this.Items.Length + 1];

            int i = 0;

            for (; i < this.Items.Length; i++)
            {
                string currentItem = this.Items[i];
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

            for (; i <= this.Items.Length; i++)
            {
                newItems[i] = this.Items[i - 1];
            }

            this.Items = newItems;

            return true;
        }
    }
}