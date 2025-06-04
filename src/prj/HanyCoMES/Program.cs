using System.Windows;

namespace HanyCoMES;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
