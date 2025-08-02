using System.Windows;
using System.Windows.Controls;
using CodeGenerator.Application.Services;

namespace CodeGenerator.Designer.UI.Controls;

public partial class SettingsControl : UserControl
{
    public SettingsControl()
    {
        this.InitializeComponent();
        this.LoadFromSettings();
    }

    private void OnLoad(object sender, RoutedEventArgs e) => this.LoadFromSettings();

    private void OnSave(object sender, RoutedEventArgs e)
    {
        Settings.Configure(this.ConnectionStringBox.Text);
        Settings.Default.Save();
        this.OnSaved(Settings.Default);
    }

    private void LoadFromSettings()
    {
        Settings.Load();
        this.ConnectionStringBox.Text = Settings.Default.ConnectionString;
        this.OnLoaded(Settings.Default);
    }

    partial void OnLoaded(Settings settings);
    partial void OnSaved(Settings settings);
}
