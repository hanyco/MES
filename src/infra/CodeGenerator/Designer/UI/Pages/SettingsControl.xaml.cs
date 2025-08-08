using System.Windows;
using System.Windows.Controls;

using CodeGenerator.Application.Services;
using CodeGenerator.UI.Dialogs;

namespace CodeGenerator.Designer.UI.Controls;

public partial class SettingsControl : UserControl
{
    private Settings _tempSettings = new() { Folders = new FolderStructure() };

    public SettingsControl()
    {
        this.InitializeComponent();
        this.LoadFromSettings();
    }

    private void OnLoad(object sender, RoutedEventArgs e) => this.LoadFromSettings();

    private void OnSave(object sender, RoutedEventArgs e)
    {
        Settings.Configure(this._tempSettings);
        Settings.Save();
    }

    private void LoadFromSettings()
    {
        Settings.Load();
        this._tempSettings = new Settings
        {
            ConnectionString = Settings.Default.ConnectionString,
            Folders = new FolderStructure
            {
                DefaultRoot = Settings.Default.Folders?.DefaultRoot,
                PagesPath = Settings.Default.Folders?.PagesPath,
                ComponentsPath = Settings.Default.Folders?.ComponentsPath,
                ViewModelsPath = Settings.Default.Folders?.ViewModelsPath,
                ControllersPath = Settings.Default.Folders?.ControllersPath,
                ApplicationPath = Settings.Default.Folders?.ApplicationPath,
                ApplicationDtosPath = Settings.Default.Folders?.ApplicationDtosPath,
                RepositoriesPath = Settings.Default.Folders?.RepositoriesPath
            }
        };

        this.ConnectionStringBox.Text = this._tempSettings.ConnectionString;
        this.RootPathBox.Text = this._tempSettings.Folders?.DefaultRoot;
        this.PagesPathBox.Text = this._tempSettings.Folders?.PagesPath;
        this.ComponentsPathBox.Text = this._tempSettings.Folders?.ComponentsPath;
        this.ViewModelsPathBox.Text = this._tempSettings.Folders?.ViewModelsPath;
        this.ControllersPathBox.Text = this._tempSettings.Folders?.ControllersPath;
        this.ApplicationPathBox.Text = this._tempSettings.Folders?.ApplicationPath;
        this.DtosPathBox.Text = this._tempSettings.Folders?.ApplicationDtosPath;
        this.RepositoriesPathBox.Text = this._tempSettings.Folders?.RepositoriesPath;
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

    private void OnConnectionStringChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.ConnectionString = this.ConnectionStringBox.Text;

    private void OnRootPathChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.Folders.DefaultRoot = this.RootPathBox.Text;

    private void OnPagesPathChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.Folders.PagesPath = this.PagesPathBox.Text;

    private void OnComponentsPathChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.Folders.ComponentsPath = this.ComponentsPathBox.Text;

    private void OnViewModelsPathChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.Folders.ViewModelsPath = this.ViewModelsPathBox.Text;

    private void OnControllersPathChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.Folders.ControllersPath = this.ControllersPathBox.Text;

    private void OnApplicationPathChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.Folders.ApplicationPath = this.ApplicationPathBox.Text;

    private void OnDtosPathChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.Folders.ApplicationDtosPath = this.DtosPathBox.Text;

    private void OnRepositoriesPathChanged(object sender, TextChangedEventArgs e)
        => this._tempSettings.Folders.RepositoriesPath = this.RepositoriesPathBox.Text;

    private void RootPathBox_LostFocus(object sender, RoutedEventArgs e)
    {

    }
}
