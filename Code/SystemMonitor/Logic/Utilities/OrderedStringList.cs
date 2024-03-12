namespace SystemMonitor.Logic.Utilities
{
    internal class OrderedStringList
    {
        public OrderedStringList(string[] items)
        {
            this.Items = items;
        }

        public string[] Items { get; private set; }

        public void AddIfNotExist(string item)
        {
            string[] newItems = new string[this.Items.Length + 1];

            int i = 0;

            for (; i < this.Items.Length; i++)
            {
                string currentItem = this.Items[i];
                int comparison = currentItem.CompareTo(item);

                if (comparison == 0)
                {
                    return;
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
        }
    }
}