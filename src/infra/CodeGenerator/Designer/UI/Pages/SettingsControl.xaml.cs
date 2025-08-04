using System.Windows;
using System.Windows.Controls;

using CodeGenerator.UI.Dialogs;

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
        var cpnnectionString = this.ConnectionStringBox.Text;
        var folders = new FolderStructure
        {
            DefaultRoot = this.RootPathBox.Text,
            PagesPath = this.PagesPathBox.Text,
            ComponentsPath = this.ComponentsPathBox.Text,
            ViewModelsPath = this.ViewModelsPathBox.Text,
            ControllersPath = this.ControllersPathBox.Text,
            ApplicationPath = this.ApplicationPathBox.Text,
            ApplicationDtosPath = this.DtosPathBox.Text,
            RepositoriesPath = this.RepositoriesPathBox.Text
        };
        Settings.Configure(cpnnectionString, folders);
        Settings.Save();
    }

    private void LoadFromSettings()
    {
        Settings.Load();
        this.ConnectionStringBox.Text = Settings.Default.ConnectionString;
        this.RootPathBox.Text = Settings.Default.Folders?.DefaultRoot;
        this.PagesPathBox.Text = Settings.Default.Folders?.PagesPath;
        this.ComponentsPathBox.Text = Settings.Default.Folders?.ComponentsPath;
        this.ViewModelsPathBox.Text = Settings.Default.Folders?.ViewModelsPath;
        this.ControllersPathBox.Text = Settings.Default.Folders?.ControllersPath;
        this.ApplicationPathBox.Text = Settings.Default.Folders?.ApplicationPath;
        this.DtosPathBox.Text = Settings.Default.Folders?.ApplicationDtosPath;
        this.RepositoriesPathBox.Text = Settings.Default.Folders?.RepositoriesPath;
    }

    private void OnBrowseRootPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.RootPathBox.Text = path;
        }
    }

    private void OnBrowseDtosPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.DtosPathBox.Text = path;
        }
    }

    private void OnBrowsePagesPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.PagesPathBox.Text = path;
        }
    }

    private void OnBrowseComponentsPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.ComponentsPathBox.Text = path;
        }
    }

    private void OnBrowseViewModelsPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.ViewModelsPathBox.Text = path;
        }
    }

    private void OnBrowseControllersPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.ControllersPathBox.Text = path;
        }
    }

    private void OnBrowseApplicationPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.ApplicationPathBox.Text = path;
        }
    }

    private void OnBrowseRepositoriesPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.RepositoriesPathBox.Text = path;
        }
    }
}
