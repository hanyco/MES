using System.Windows;

namespace CodeGenerator;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnManageDto(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("مدیریت DTO انتخاب شد");
    }

    private void OnManageCqrs(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("مدیریت CQRS انتخاب شد");
    }

    private void OnSettings(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("تنظیمات انتخاب شد");
    }
}
