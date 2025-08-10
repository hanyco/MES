using System.Windows;
using System.Windows.Controls;

using CodeGenerator.UI.Dialogs;

namespace CodeGenerator.Designer.UI.Controls;

public partial class SettingsControl : UserControl
{
    private readonly Settings _settings = Settings.Default;
    public SettingsControl() =>
        this.InitializeComponent();

    private void OnLoad(object sender, RoutedEventArgs e) =>
        this.LoadFromSettings();

    private void OnSave(object sender, RoutedEventArgs e)
    {
        this._settings.Folders.DefaultRoot = this.RootPathBox.Text;
        this._settings.Folders.PagesPath = this.PagesPathBox.Text;
        this._settings.Folders.ComponentsPath = this.ComponentsPathBox.Text;
        this._settings.Folders.ViewModelsPath = this.ViewModelsPathBox.Text;
        this._settings.Folders.ControllersPath = this.ControllersPathBox.Text;
        this._settings.Folders.ApplicationPath = this.ApplicationPathBox.Text;
        this._settings.Folders.ApplicationDtosPath = this.DtosPathBox.Text;
        this._settings.Folders.RepositoriesPath = this.RepositoriesPathBox.Text;
        Settings.Configure(this._settings);
        Settings.Save();
    }

    private void LoadFromSettings()
    {
        Settings.Load();
        this.RootPathBox.Text = this._settings.Folders.DefaultRoot;
        this.PagesPathBox.Text = this._settings.Folders.PagesPath;
        this.ComponentsPathBox.Text = this._settings.Folders.ComponentsPath;
        this.ViewModelsPathBox.Text = this._settings.Folders.ViewModelsPath;
        this.ControllersPathBox.Text = this._settings.Folders.ControllersPath;
        this.ApplicationPathBox.Text = this._settings.Folders.ApplicationPath;
        this.DtosPathBox.Text = this._settings.Folders.ApplicationDtosPath;
        this.RepositoriesPathBox.Text = this._settings.Folders.RepositoriesPath;
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
