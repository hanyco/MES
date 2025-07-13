using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CodeGenerator.Designer.UI.Common;

public static class DataGridExtensions
{
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(IList),
            typeof(DataGridExtensions),
            new PropertyMetadata(null, OnSelectedItemsChanged));

    public static void SetSelectedItems(DependencyObject element, IList? value) =>
        element.SetValue(SelectedItemsProperty, value);

    public static IList? GetSelectedItems(DependencyObject element) =>
        (IList?)element.GetValue(SelectedItemsProperty);

    private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid grid)
        {
            return;
        }

        grid.SelectionChanged -= GridOnSelectionChanged;

        if (e.NewValue is IList list)
        {
            grid.SelectedItems.Clear();
            foreach (var item in list)
            {
                grid.SelectedItems.Add(item);
            }

            grid.SelectionChanged += GridOnSelectionChanged;
        }
    }

    private static void GridOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var grid = (DataGrid)sender;
        if (GetSelectedItems(grid) is IList list)
        {
            list.Clear();
            foreach (var item in grid.SelectedItems.Cast<object>())
            {
                list.Add(item);
            }
        }
    }
}
