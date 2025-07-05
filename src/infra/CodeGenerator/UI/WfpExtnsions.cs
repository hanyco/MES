using System.Windows.Controls;

namespace CodeGenerator.UI;

public static class WfpExtnsion
{
    extension(ItemCollection items)
    {
        public TItem FindOrAdd<TItem>(Func<TItem, bool> find, Func<TItem> create)
        {
            foreach (var item in items)
            {
                if (item is TItem existing && find(existing))
                {
                    return existing;
                }
            }
            var newItem = create();
            _ = items.Add(newItem);
            return newItem;
        }
    }

    extension(TreeView @this)
    {
        public TItem? GetSelectedItem<TItem>()
            => @this.SelectedItem is TItem item
                ? item
                : @this.SelectedItem is TreeViewItem { Tag: TItem tagItem }
                    ? tagItem
                    : default;
    }
}