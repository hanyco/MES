namespace CodeGenerator.Designer.UI.Common;

using System.Collections;
using System.Windows.Controls;
using System.Windows;

public static class DataGridSelectedItemsBehavior
{
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
        "SelectedItems",
        typeof(IList),
        typeof(DataGridSelectedItemsBehavior),
        new PropertyMetadata(null, OnSelectedItemsChanged));

    public static IList? GetSelectedItems(DataGrid element) => (IList?)element.GetValue(SelectedItemsProperty);
    public static void SetSelectedItems(DataGrid element, IList? value) => element.SetValue(SelectedItemsProperty, value);

    private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid grid)
        {
            grid.SelectionChanged -= Grid_SelectionChanged;
            if (e.NewValue is IList list)
            {
                grid.SelectedItems.Clear();
                foreach (var item in list)
                {
                    grid.SelectedItems.Add(item);
                }
                grid.SelectionChanged += Grid_SelectionChanged;
            }
        }
    }

    private static void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is DataGrid grid && GetSelectedItems(grid) is IList boundList)
        {
            foreach (var item in e.RemovedItems)
            {
                boundList.Remove(item);
            }
            foreach (var item in e.AddedItems)
            {
                boundList.Add(item);
            }
        }
    }
}
