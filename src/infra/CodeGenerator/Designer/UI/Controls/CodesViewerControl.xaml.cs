using System.Windows;
using System.Windows.Controls;

using Library.CodeGenLib.Models;

namespace CodeGenerator.Designer.UI.Controls;

public partial class CodesViewerControl : UserControl
{
    public CodesViewerControl() => this.InitializeComponent();

    public Codes Codes
    {
        get => (Codes)this.GetValue(CodesProperty);
        set => this.SetValue(CodesProperty, value);
    }

    public static readonly DependencyProperty CodesProperty =
        DependencyProperty.Register(
            nameof(Codes),
            typeof(Codes),
            typeof(CodesViewerControl),
            new PropertyMetadata(Codes.Empty, OnCodesPropertyChanged));

    private static void OnCodesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CodesViewerControl control && e.NewValue is Codes codes)
        {
            control.UpdateTabs(codes);
        }
    }

    private void UpdateTabs(Codes codes)
    {
        this.Tabs.Items.Clear();
        foreach (var code in codes)
        {
            if (code is null)
            {
                continue;
            }

            var box = new TextBox
            {
                Text = code.Statement,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                IsReadOnly = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = double.NaN,
                Height = double.NaN
            };
            var tab = new TabItem
            {
                Header = code.Name,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                Content = box
            };
            this.Tabs.Items.Add(tab);
        }
        this.OnCodesChanged(codes);
    }

    partial void OnCodesChanged(Codes codes);
}
